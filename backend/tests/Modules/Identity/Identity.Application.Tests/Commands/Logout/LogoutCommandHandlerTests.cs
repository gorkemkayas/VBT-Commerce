using Identity.Application.Abstractions;
using Identity.Application.Commands.Logout;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Identity.Application.Tests.Commands.Logout;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidActiveToken_RevokesTokenAndSaves()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), ClientPlatform.Web, Guid.NewGuid(), "hashed-token", DateTime.UtcNow.AddDays(7));
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-token")).Returns("hashed-token");

        var handler = new LogoutCommandHandler(dbContext, tokenService.Object);
        var command = new LogoutCommand("raw-token");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        var stored = await dbContext.RefreshTokens.FirstAsync(t => t.Id == refreshToken.Id);
        stored.IsRevoked.Should().BeTrue();
        stored.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithAlreadyRevokedToken_DoesNotThrowAndLeavesTokenRevoked()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), ClientPlatform.Web, Guid.NewGuid(), "hashed-token", DateTime.UtcNow.AddDays(7));
        refreshToken.Revoke();
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var revokedAtBefore = refreshToken.RevokedAt;

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-token")).Returns("hashed-token");

        var handler = new LogoutCommandHandler(dbContext, tokenService.Object);
        var command = new LogoutCommand("raw-token");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        var stored = await dbContext.RefreshTokens.FirstAsync(t => t.Id == refreshToken.Id);
        stored.IsRevoked.Should().BeTrue();
        stored.RevokedAt.Should().Be(revokedAtBefore);
    }

    [Fact]
    public async Task Handle_WithUnknownToken_ReturnsUnitWithoutError()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("unknown-token")).Returns("unknown-hash");

        var handler = new LogoutCommandHandler(dbContext, tokenService.Object);
        var command = new LogoutCommand("unknown-token");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}
