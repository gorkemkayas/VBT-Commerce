using Identity.Application.Abstractions;
using Identity.Application.Commands.Login;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Identity.Application.Tests.Commands.Login;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsAuthResultAndPersistsRefreshToken()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "stored-hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(h => h.Verify("Password123", "stored-hash")).Returns(true);

        var tokenService = new Mock<ITokenService>();
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(15);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(7);
        tokenService.Setup(t => t.GenerateAccessToken(It.Is<User>(u => u.Id == user.Id)))
            .Returns(("access-token", accessExpiresAt));
        tokenService.Setup(t => t.GenerateRefreshToken(ClientPlatform.Web))
            .Returns(("raw-refresh-token", "refresh-token-hash", refreshExpiresAt));

        var handler = new LoginCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new LoginCommand("jane@example.com", "Password123", ClientPlatform.Web);

        var result = await handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("raw-refresh-token");

        var storedRefreshToken = dbContext.RefreshTokens.FirstOrDefault(t => t.UserId == user.Id);
        storedRefreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ThrowsInvalidCredentialsException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();

        var handler = new LoginCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new LoginCommand("unknown@example.com", "Password123", ClientPlatform.Web);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsInvalidCredentialsException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "stored-hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(h => h.Verify("WrongPassword", "stored-hash")).Returns(false);
        var tokenService = new Mock<ITokenService>();

        var handler = new LoginCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new LoginCommand("jane@example.com", "WrongPassword", ClientPlatform.Web);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNullPasswordHash_ThrowsInvalidCredentialsException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "temp-hash", "Jane", "Doe");
        // Simulate an account with no usable password hash (e.g. external-auth-only account).
        user.SetPasswordHash(null!);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();

        var handler = new LoginCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new LoginCommand("jane@example.com", "Password123", ClientPlatform.Web);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => handler.Handle(command, CancellationToken.None));

        passwordHasher.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
