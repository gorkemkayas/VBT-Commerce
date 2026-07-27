using Identity.Application.Abstractions;
using Identity.Application.Commands.ForgotPassword;
using Identity.Contracts.Events;
using Identity.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Identity.Application.Tests.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingEmail_CreatesResetTokenAndPublishesEvent()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();
        var user = User.Register("jane@example.com", "hash", "Jane", "Doe");
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var tokenService = new Mock<ITokenService>();
        var expiresAt = DateTime.UtcNow.AddHours(1);
        tokenService.Setup(t => t.GeneratePasswordResetToken())
            .Returns(("raw-reset-token", "reset-token-hash", expiresAt));

        var publisher = new Mock<IPublisher>();

        var handler = new ForgotPasswordCommandHandler(dbContext, tokenService.Object, publisher.Object);
        var command = new ForgotPasswordCommand("Jane@Example.com");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);

        var storedToken = await dbContext.PasswordResetTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
        storedToken.Should().NotBeNull();
        storedToken!.TokenHash.Should().Be("reset-token-hash");

        publisher.Verify(p => p.Publish(
            It.Is<PasswordResetRequestedEvent>(e => e.UserId == user.Id && e.Email == user.Email && e.RawToken == "raw-reset-token"),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ReturnsUnitWithoutCreatingTokenOrPublishing()
    {
        using var dbContext = TestIdentityDbContextFactory.Create();

        var tokenService = new Mock<ITokenService>();
        var publisher = new Mock<IPublisher>();

        var handler = new ForgotPasswordCommandHandler(dbContext, tokenService.Object, publisher.Object);
        var command = new ForgotPasswordCommand("unknown@example.com");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        (await dbContext.PasswordResetTokens.AnyAsync()).Should().BeFalse();
        publisher.Verify(p => p.Publish(It.IsAny<PasswordResetRequestedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        tokenService.Verify(t => t.GeneratePasswordResetToken(), Times.Never);
    }
}
