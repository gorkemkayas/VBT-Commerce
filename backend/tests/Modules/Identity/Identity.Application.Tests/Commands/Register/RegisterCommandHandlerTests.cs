using Identity.Application.Abstractions;
using Identity.Application.Commands.Register;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Identity.Application.Tests.Commands.Register;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNewEmail_CreatesUserAndRefreshTokenAndReturnsAuthResult()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(h => h.Hash("Password123")).Returns("hashed-password");

        var tokenService = new Mock<ITokenService>();
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(15);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(7);
        tokenService.Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("access-token", accessExpiresAt));
        tokenService.Setup(t => t.GenerateRefreshToken(ClientPlatform.Web))
            .Returns(("raw-refresh-token", "refresh-token-hash", refreshExpiresAt));

        var handler = new RegisterCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new RegisterCommand("new@example.com", "Password123", "Jane", "Doe", ClientPlatform.Web);

        var result = await handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("raw-refresh-token");
        result.AccessTokenExpiresAt.Should().Be(accessExpiresAt);

        var storedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "new@example.com");
        storedUser.Should().NotBeNull();
        storedUser!.PasswordHash.Should().Be("hashed-password");
        storedUser.FirstName.Should().Be("Jane");
        storedUser.LastName.Should().Be("Doe");

        var storedRefreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == storedUser.Id);
        storedRefreshToken.Should().NotBeNull();
        storedRefreshToken!.TokenHash.Should().Be("refresh-token-hash");
    }

    [Fact]
    public async Task Handle_WithNormalizesEmailCase_StoresLowercasedEmail()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));
        tokenService.Setup(t => t.GenerateRefreshToken(It.IsAny<ClientPlatform>()))
            .Returns(("raw-refresh-token", "refresh-token-hash", DateTime.UtcNow.AddDays(7)));

        var handler = new RegisterCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new RegisterCommand(" New@Example.com ", "Password123", "Jane", "Doe", ClientPlatform.Web);

        await handler.Handle(command, CancellationToken.None);

        var storedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "new@example.com");
        storedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsEmailAlreadyRegisteredException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        dbContext.Users.Add(User.Register("existing@example.com", "some-hash", "John", "Smith"));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();

        var handler = new RegisterCommandHandler(dbContext, passwordHasher.Object, tokenService.Object);
        var command = new RegisterCommand("Existing@Example.com", "Password123", "Jane", "Doe", ClientPlatform.Web);

        await Assert.ThrowsAsync<EmailAlreadyRegisteredException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
