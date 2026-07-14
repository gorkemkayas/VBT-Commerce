using Identity.Application.Abstractions;
using Identity.Application.Common;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.RefreshTokens;

public class RefreshTokenCommandHandler(
    IIdentityDbContext dbContext,
    ITokenService tokenService,
    IRefreshTokenLockProvider lockProvider) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = tokenService.HashToken(request.RefreshToken);

        var tokenEntity = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, cancellationToken);

        if (tokenEntity is null || tokenEntity.IsExpired)
            throw new InvalidRefreshTokenException("Invalid refresh token.");

        var semaphore = lockProvider.GetLock(tokenEntity.UserId, tokenEntity.Platform);
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            // Re-read the current state after acquiring the lock (source of truth for concurrent requests)
            tokenEntity = await dbContext.RefreshTokens.FirstAsync(t => t.Id == tokenEntity.Id, cancellationToken);

            if (tokenEntity.IsRevoked)
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
        finally
        {
            semaphore.Release();
        }
    }
}
