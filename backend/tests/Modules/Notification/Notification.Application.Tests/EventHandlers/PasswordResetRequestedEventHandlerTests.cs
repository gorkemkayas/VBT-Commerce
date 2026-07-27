using FluentAssertions;
using Identity.Contracts.Events;
using Moq;
using Notification.Application.Email;
using Notification.Application.EventHandlers;
using Notification.Application.Links;
using Xunit;

namespace Notification.Application.Tests.EventHandlers;

public class PasswordResetRequestedEventHandlerTests
{
    private readonly Mock<IEmailSender> _emailSender = new();
    private readonly Mock<IPasswordResetLinkBuilder> _linkBuilder = new();

    private PasswordResetRequestedEventHandler CreateHandler(Notification.Infrastructure.Persistence.NotificationDbContext dbContext) =>
        new(_emailSender.Object, _linkBuilder.Object, dbContext);

    [Fact]
    public async Task Handle_WithSuccessfulSend_LogsSuccessWithBuiltLink()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var @event = new PasswordResetRequestedEvent(userId, "user@example.com", "raw-token");

        _linkBuilder.Setup(b => b.Build("raw-token")).Returns("https://app.example.com/reset?token=raw-token");
        _emailSender
            .Setup(s => s.SendAsync("user@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(true, null));

        var handler = CreateHandler(dbContext);

        await handler.Handle(@event, CancellationToken.None);

        var log = dbContext.NotificationLogs.Single();
        log.NotificationType.Should().Be("PasswordResetRequested");
        log.ReferenceId.Should().Be(userId);
        log.RecipientEmail.Should().Be("user@example.com");
        log.Body.Should().Contain("https://app.example.com/reset?token=raw-token");
        log.IsSuccess.Should().BeTrue();
        log.ErrorMessage.Should().BeNull();

        _linkBuilder.Verify(b => b.Build("raw-token"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSendFails_LogsFailureWithoutThrowing()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var @event = new PasswordResetRequestedEvent(Guid.NewGuid(), "user@example.com", "raw-token");

        _linkBuilder.Setup(b => b.Build("raw-token")).Returns("https://app.example.com/reset?token=raw-token");
        _emailSender
            .Setup(s => s.SendAsync("user@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(false, "SMTP unreachable"));

        var handler = CreateHandler(dbContext);

        await handler.Handle(@event, CancellationToken.None);

        var log = dbContext.NotificationLogs.Single();
        log.IsSuccess.Should().BeFalse();
        log.ErrorMessage.Should().Be("SMTP unreachable");
    }
}
