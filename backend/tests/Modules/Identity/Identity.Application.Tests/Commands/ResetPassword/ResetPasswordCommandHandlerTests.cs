using Identity.Application.Abstractions;
using Identity.Application.Commands.ResetPassword;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Identity.Application.Tests.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidToken_UpdatesPasswordAndMarksTokenUsed()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "old-hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        var resetToken = PasswordResetToken.Create(user.Id, "reset-token-hash", DateTime.UtcNow.AddHours(1));
        dbContext.PasswordResetTokens.Add(resetToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-reset-token")).Returns("reset-token-hash");

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(h => h.Hash("NewPassword123")).Returns("new-hash");

        var handler = new ResetPasswordCommandHandler(dbContext, tokenService.Object, passwordHasher.Object);
        var command = new ResetPasswordCommand("raw-reset-token", "NewPassword123");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);

        var storedUser = await dbContext.Users.FirstAsync(u => u.Id == user.Id);
        storedUser.PasswordHash.Should().Be("new-hash");

        var storedToken = await dbContext.PasswordResetTokens.FirstAsync(t => t.Id == resetToken.Id);
        storedToken.IsUsed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentToken_ThrowsInvalidOrExpiredResetTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken(It.IsAny<string>())).Returns("unknown-hash");
        var passwordHasher = new Mock<IPasswordHasher>();

        var handler = new ResetPasswordCommandHandler(dbContext, tokenService.Object, passwordHasher.Object);
        var command = new ResetPasswordCommand("unknown-token", "NewPassword123");

        await Assert.ThrowsAsync<InvalidOrExpiredResetTokenException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyUsedToken_ThrowsInvalidOrExpiredResetTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "old-hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        var resetToken = PasswordResetToken.Create(user.Id, "reset-token-hash", DateTime.UtcNow.AddHours(1));
        resetToken.MarkUsed();
        dbContext.PasswordResetTokens.Add(resetToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-reset-token")).Returns("reset-token-hash");
        var passwordHasher = new Mock<IPasswordHasher>();

        var handler = new ResetPasswordCommandHandler(dbContext, tokenService.Object, passwordHasher.Object);
        var command = new ResetPasswordCommand("raw-reset-token", "NewPassword123");

        await Assert.ThrowsAsync<InvalidOrExpiredResetTokenException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ThrowsInvalidOrExpiredResetTokenException()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "old-hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        var resetToken = PasswordResetToken.Create(user.Id, "reset-token-hash", DateTime.UtcNow.AddHours(-1));
        dbContext.PasswordResetTokens.Add(resetToken);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashToken("raw-reset-token")).Returns("reset-token-hash");
        var passwordHasher = new Mock<IPasswordHasher>();

        var handler = new ResetPasswordCommandHandler(dbContext, tokenService.Object, passwordHasher.Object);
        var command = new ResetPasswordCommand("raw-reset-token", "NewPassword123");

        await Assert.ThrowsAsync<InvalidOrExpiredResetTokenException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
