using Identity.Application.Abstractions;
using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.Register;

public class RegisterCommandHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var alreadyExists = await dbContext.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (alreadyExists)
            throw new EmailAlreadyRegisteredException(request.Email);

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Register(normalizedEmail, passwordHash, request.FirstName, request.LastName);
        dbContext.Users.Add(user);

        var (accessToken, accessTokenExpiresAt) = tokenService.GenerateAccessToken(user);
        var (rawRefreshToken, refreshTokenHash, refreshTokenExpiresAt) = tokenService.GenerateRefreshToken(request.Platform);

        var refreshToken = RefreshToken.Create(user.Id, request.Platform, Guid.NewGuid(), refreshTokenHash, refreshTokenExpiresAt);
        dbContext.RefreshTokens.Add(refreshToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken, rawRefreshToken, accessTokenExpiresAt);
    }
}
