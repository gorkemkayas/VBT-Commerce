using Identity.Application.Abstractions;
using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.Login;

public class LoginCommandHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null || user.PasswordHash is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var (accessToken, accessTokenExpiresAt) = tokenService.GenerateAccessToken(user);
        var (rawRefreshToken, refreshTokenHash, refreshTokenExpiresAt) = tokenService.GenerateRefreshToken(request.Platform);

        var refreshToken = RefreshToken.Create(user.Id, request.Platform, Guid.NewGuid(), refreshTokenHash, refreshTokenExpiresAt);
        dbContext.RefreshTokens.Add(refreshToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken, rawRefreshToken, accessTokenExpiresAt);
    }
}
