using Identity.Application.Abstractions;
using Identity.Application.Commands.RefreshTokens;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Identity.Application.Tests.Commands.RefreshTokens;

public class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidToken_RotatesTokenAndReturnsAuthResult()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        var familyId = Guid.NewGuid();
        var refreshToken = RefreshToken.Create(user.Id, ClientPlatform.Web, familyId, "old-hash", DateTime.UtcNow.AddDays(7));
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-old-token")).Returns("old-hash");
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(15);
        var newRefreshExpiresAt = DateTime.UtcNow.AddDays(7);
        tokenService.Setup(t => t.GenerateAccessToken(It.Is<User>(u => u.Id == user.Id)))
            .Returns(("access-token", accessExpiresAt));
        tokenService.Setup(t => t.GenerateRefreshToken(ClientPlatform.Web))
            .Returns(("raw-new-token", "new-hash", newRefreshExpiresAt));

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("raw-old-token", ClientPlatform.Web);

        var result = await handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("raw-new-token");

        var oldTokenAfter = await dbContext.RefreshTokens.FirstAsync(t => t.Id == refreshToken.Id);
        oldTokenAfter.IsRevoked.Should().BeTrue();
        oldTokenAfter.ReplacedByTokenId.Should().NotBeNull();

        var newToken = await dbContext.RefreshTokens.FirstAsync(t => t.Id == oldTokenAfter.ReplacedByTokenId!.Value);
        newToken.TokenHash.Should().Be("new-hash");
        newToken.FamilyId.Should().Be(familyId);
    }

    [Fact]
    public async Task Handle_WithNonExistentToken_ThrowsInvalidRefreshTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken(It.IsAny<string>())).Returns("unknown-hash");

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("unknown-token", ClientPlatform.Web);

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ThrowsInvalidRefreshTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), ClientPlatform.Web, Guid.NewGuid(), "expired-hash", DateTime.UtcNow.AddDays(-1));
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-token")).Returns("expired-hash");

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("raw-token", ClientPlatform.Web);

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithRevokedTokenOutsideGraceWindowAndNoReplacement_ThrowsInvalidRefreshTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), ClientPlatform.Web, Guid.NewGuid(), "revoked-hash", DateTime.UtcNow.AddDays(7));
        refreshToken.Revoke();
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-token")).Returns("revoked-hash");

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("raw-token", ClientPlatform.Web);

        var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("This refresh token has already been used.");
    }

    [Fact]
    public async Task Handle_WithRevokedTokenWithinGraceWindow_FollowsChainAndSucceeds()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "hash", "Jane", "Doe");
        dbContext.Users.Add(user);

        var familyId = Guid.NewGuid();
        var replacement = RefreshToken.Create(user.Id, ClientPlatform.Web, familyId, "replacement-hash", DateTime.UtcNow.AddDays(7));
        dbContext.RefreshTokens.Add(replacement);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var original = RefreshToken.Create(user.Id, ClientPlatform.Web, familyId, "original-hash", DateTime.UtcNow.AddDays(7));
        original.Revoke(replacement.Id); // revoked "just now" - within the 30s grace window
        dbContext.RefreshTokens.Add(original);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-original-token")).Returns("original-hash");
        tokenService.Setup(t => t.GenerateAccessToken(It.Is<User>(u => u.Id == user.Id)))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));
        tokenService.Setup(t => t.GenerateRefreshToken(ClientPlatform.Web))
            .Returns(("raw-new-token", "new-hash", DateTime.UtcNow.AddDays(7)));

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("raw-original-token", ClientPlatform.Web);

        var result = await handler.Handle(command, CancellationToken.None);

        result.RefreshToken.Should().Be("raw-new-token");

        var replacementAfter = await dbContext.RefreshTokens.FirstAsync(t => t.Id == replacement.Id);
        replacementAfter.IsRevoked.Should().BeTrue();
        replacementAfter.ReplacedByTokenId.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithRevokedTokenChainLeadingToExpiredReplacement_ThrowsInvalidRefreshTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "hash", "Jane", "Doe");
        dbContext.Users.Add(user);

        var familyId = Guid.NewGuid();
        var replacement = RefreshToken.Create(user.Id, ClientPlatform.Web, familyId, "replacement-hash", DateTime.UtcNow.AddDays(-1));
        dbContext.RefreshTokens.Add(replacement);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var original = RefreshToken.Create(user.Id, ClientPlatform.Web, familyId, "original-hash", DateTime.UtcNow.AddDays(7));
        original.Revoke(replacement.Id);
        dbContext.RefreshTokens.Add(original);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-original-token")).Returns("original-hash");

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("raw-original-token", ClientPlatform.Web);

        var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("This refresh token has already been used.");
    }

    [Fact]
    public async Task Handle_WithUserNotFound_ThrowsInvalidRefreshTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), ClientPlatform.Web, Guid.NewGuid(), "orphan-hash", DateTime.UtcNow.AddDays(7));
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-token")).Returns("orphan-hash");

        var handler = new RefreshTokenCommandHandler(dbContext, tokenService.Object);
        var command = new RefreshTokenCommand("raw-token", ClientPlatform.Web);

        var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("User not found.");
    }
}
