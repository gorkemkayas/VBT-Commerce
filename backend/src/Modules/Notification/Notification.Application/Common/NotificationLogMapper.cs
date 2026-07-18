using Notification.Domain.Entities;

namespace Notification.Application.Common;

public static class NotificationLogMapper
{
    public static NotificationLogDto ToDto(NotificationLog log) => new(
        log.Id, log.NotificationType, log.ReferenceId, log.RecipientEmail, log.Subject, log.IsSuccess, log.ErrorMessage, log.CreatedAt);
}
