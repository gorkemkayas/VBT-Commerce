using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Abstractions;
using Notification.Application.Email;
using Notification.Application.Integrations;
using Notification.Domain.Entities;
using Order.Contracts.Events;

namespace Notification.Application.EventHandlers;

/// <summary>
/// Subscribed to Order's OrderConfirmedEvent, published by OrderOutboxProcessor — never invoked
/// directly from a request. A failed send is recorded (IsSuccess=false) rather than rethrown: a
/// permanently bad/unreachable recipient address shouldn't make OrderOutboxProcessor retry this
/// outbox row forever, and NotificationLog itself is already the audit/visibility record for it.
/// </summary>
public class OrderConfirmedEventHandler(
    ICustomerIntegrationService customerIntegrationService,
    IIdentityIntegrationService identityIntegrationService,
    IEmailSender emailSender,
    INotificationDbContext dbContext,
    ILogger<OrderConfirmedEventHandler> logger)
    : INotificationHandler<OrderConfirmedEvent>
{
    private const string NotificationType = "OrderConfirmed";

    public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
    {
        var email = notification.UserId is not null
            ? await identityIntegrationService.GetUserEmailAsync(notification.UserId.Value, cancellationToken)
            : await customerIntegrationService.GetGuestCustomerEmailAsync(notification.GuestCustomerId!.Value, cancellationToken);

        if (email is null)
        {
            logger.LogWarning(
                "Could not resolve a recipient email for order '{OrderId}' (UserId={UserId}, GuestCustomerId={GuestCustomerId}); skipping notification.",
                notification.OrderId, notification.UserId, notification.GuestCustomerId);
            return;
        }

        const string subject = "Siparişiniz Onaylandı";
        var body = $"Merhaba,\n\n#{notification.OrderId} numaralı siparişiniz onaylandı ve hazırlanmaya başlandı.\n\nTeşekkür ederiz.";

        var result = await emailSender.SendAsync(email, subject, body, cancellationToken);

        dbContext.NotificationLogs.Add(
            NotificationLog.Create(NotificationType, notification.OrderId, email, subject, body, result.IsSuccess, result.ErrorMessage));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
