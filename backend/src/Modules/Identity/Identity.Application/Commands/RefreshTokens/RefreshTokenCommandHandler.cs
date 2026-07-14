using Identity.Application.Abstractions;
using Identity.Application.Common;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.RefreshTokens;

public class RefreshTokenCommandHandler(
    IIdentityDbContext dbContext,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private static readonly TimeSpan GraceWindow = TimeSpan.FromSeconds(30);

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = tokenService.HashToken(request.RefreshToken);

        var tokenEntity = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, cancellationToken);

        if (tokenEntity is null || tokenEntity.IsExpired)
            throw new InvalidRefreshTokenException("Invalid refresh token.");

        // Grace window: a concurrent request may have already rotated this exact token
        // (e.g. two browser tabs racing on the same cookie). Instead of failing, follow the
        // rotation chain forward and issue a fresh pair from the currently active token —
        // as long as the rotation that revoked it happened moments ago. Safe to read straight
        // from this fresh DbContext: RefreshTokenConcurrencyBehavior guarantees this handler
        // only starts running after any concurrent sibling request's transaction has committed.
        while (tokenEntity.IsRevoked)
        {
            var withinGraceWindow = tokenEntity.ReplacedByTokenId is not null
                && tokenEntity.RevokedAt is not null
                && DateTime.UtcNow - tokenEntity.RevokedAt.Value < GraceWindow;

            if (!withinGraceWindow)
                throw new InvalidRefreshTokenException("This refresh token has already been used.");

            tokenEntity = await dbContext.RefreshTokens
                .FirstAsync(t => t.Id == tokenEntity.ReplacedByTokenId!.Value, cancellationToken);
        }

        if (tokenEntity.IsExpired)
            throw new InvalidRefreshTokenException("This refresh token has already been used.");

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == tokenEntity.UserId, cancellationToken)
            ?? throw new InvalidRefreshTokenException("User not found.");

        var (accessToken, accessTokenExpiresAt) = tokenService.GenerateAccessToken(user);
        var (rawRefreshToken, refreshTokenHash, refreshTokenExpiresAt) = tokenService.GenerateRefreshToken(tokenEntity.Platform);

        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            user.Id, tokenEntity.Platform, tokenEntity.FamilyId, refreshTokenHash, refreshTokenExpiresAt);

        dbContext.RefreshTokens.Add(newRefreshToken);
        tokenEntity.Revoke(newRefreshToken.Id);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken, rawRefreshToken, accessTokenExpiresAt);
    }
}
