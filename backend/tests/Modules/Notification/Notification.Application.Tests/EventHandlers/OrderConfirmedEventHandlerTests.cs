using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Notification.Application.Email;
using Notification.Application.EventHandlers;
using Notification.Application.Integrations;
using Order.Contracts.Events;
using Xunit;

namespace Notification.Application.Tests.EventHandlers;

public class OrderConfirmedEventHandlerTests
{
    private readonly Mock<ICustomerIntegrationService> _customerIntegrationService = new();
    private readonly Mock<IIdentityIntegrationService> _identityIntegrationService = new();
    private readonly Mock<IEmailSender> _emailSender = new();
    private readonly Mock<ILogger<OrderConfirmedEventHandler>> _logger = new();

    private OrderConfirmedEventHandler CreateHandler(Notification.Infrastructure.Persistence.NotificationDbContext dbContext) =>
        new(_customerIntegrationService.Object, _identityIntegrationService.Object, _emailSender.Object, dbContext, _logger.Object);

    [Fact]
    public async Task Handle_WithRegisteredUser_ResolvesEmailViaIdentityAndLogsSuccess()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var @event = new OrderConfirmedEvent(orderId, userId, null);

        _identityIntegrationService
            .Setup(s => s.GetUserEmailAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("user@example.com");
        _emailSender
            .Setup(s => s.SendAsync("user@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(true, null));

        var handler = CreateHandler(dbContext);

        await handler.Handle(@event, CancellationToken.None);

        var log = dbContext.NotificationLogs.Single();
        log.NotificationType.Should().Be("OrderConfirmed");
        log.ReferenceId.Should().Be(orderId);
        log.RecipientEmail.Should().Be("user@example.com");
        log.IsSuccess.Should().BeTrue();
        log.ErrorMessage.Should().BeNull();

        _customerIntegrationService.Verify(
            s => s.GetGuestCustomerEmailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailSender.Verify(
            s => s.SendAsync("user@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithGuestCustomer_ResolvesEmailViaCustomerIntegration()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var @event = new OrderConfirmedEvent(orderId, null, guestCustomerId);

        _customerIntegrationService
            .Setup(s => s.GetGuestCustomerEmailAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("guest@example.com");
        _emailSender
            .Setup(s => s.SendAsync("guest@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(true, null));

        var handler = CreateHandler(dbContext);

        await handler.Handle(@event, CancellationToken.None);

        var log = dbContext.NotificationLogs.Single();
        log.RecipientEmail.Should().Be("guest@example.com");
        log.IsSuccess.Should().BeTrue();

        _identityIntegrationService.Verify(
            s => s.GetUserEmailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailCannotBeResolved_DoesNotSendOrLogAndSkips()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var @event = new OrderConfirmedEvent(Guid.NewGuid(), userId, null);

        _identityIntegrationService
            .Setup(s => s.GetUserEmailAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = CreateHandler(dbContext);

        await handler.Handle(@event, CancellationToken.None);

        dbContext.NotificationLogs.Should().BeEmpty();
        _emailSender.Verify(
            s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSendFails_LogsFailureWithoutThrowing()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var @event = new OrderConfirmedEvent(orderId, userId, null);

        _identityIntegrationService
            .Setup(s => s.GetUserEmailAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("user@example.com");
        _emailSender
            .Setup(s => s.SendAsync("user@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(false, "SMTP timeout"));

        var handler = CreateHandler(dbContext);

        await handler.Handle(@event, CancellationToken.None);

        var log = dbContext.NotificationLogs.Single();
        log.IsSuccess.Should().BeFalse();
        log.ErrorMessage.Should().Be("SMTP timeout");
    }
}
