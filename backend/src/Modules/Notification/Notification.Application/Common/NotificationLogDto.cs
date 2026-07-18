namespace Notification.Application.Common;

public record NotificationLogDto(
    Guid Id,
    string NotificationType,
    Guid ReferenceId,
    string RecipientEmail,
    string Subject,
    bool IsSuccess,
    string? ErrorMessage,
    DateTime CreatedAt);
