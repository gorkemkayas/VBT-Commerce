using Identity.Contracts.Events;
using MediatR;
using Notification.Application.Abstractions;
using Notification.Application.Email;
using Notification.Application.Links;
using Notification.Domain.Entities;

namespace Notification.Application.EventHandlers;

/// <summary>
/// Subscribed to Identity's PasswordResetRequestedEvent, published synchronously (no Outbox — see
/// the event's own doc comment) directly from ForgotPasswordCommandHandler. Same failure-handling
/// shape as OrderConfirmedEventHandler: a send failure is recorded, not rethrown.
/// </summary>
public class PasswordResetRequestedEventHandler(
    IEmailSender emailSender,
    IPasswordResetLinkBuilder linkBuilder,
    INotificationDbContext dbContext)
    : INotificationHandler<PasswordResetRequestedEvent>
{
    private const string NotificationType = "PasswordResetRequested";

    public async Task Handle(PasswordResetRequestedEvent notification, CancellationToken cancellationToken)
    {
        var resetLink = linkBuilder.Build(notification.RawToken);

        const string subject = "Şifre Sıfırlama Talebi";
        var body = $"Merhaba,\n\nŞifrenizi sıfırlamak için aşağıdaki linke tıklayın:\n{resetLink}\n\n" +
                   "Bu talebi siz yapmadıysanız bu e-postayı görmezden gelebilirsiniz.";

        var result = await emailSender.SendAsync(notification.Email, subject, body, cancellationToken);

        dbContext.NotificationLogs.Add(
            NotificationLog.Create(NotificationType, notification.UserId, notification.Email, subject, body, result.IsSuccess, result.ErrorMessage));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
