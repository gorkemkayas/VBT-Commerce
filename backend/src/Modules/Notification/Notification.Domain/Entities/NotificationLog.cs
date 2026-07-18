namespace Notification.Domain.Entities;

/// <summary>
/// A write-once audit record of a single notification attempt — created already in its final state
/// (IsSuccess/ErrorMessage known at construction time) rather than as a mutable pending-then-updated
/// row, since the actual send attempt already happened by the time the handler builds this. Recipient
/// email is denormalized here (not just a UserId/GuestCustomerId reference) because the address used
/// at send time is what matters for the audit trail, even if the user later changes their email.
/// </summary>
public class NotificationLog
{
    public Guid Id { get; private set; }
    public string NotificationType { get; private set; } = null!;
    public Guid ReferenceId { get; private set; }
    public string RecipientEmail { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private NotificationLog()
    {
    }

    public static NotificationLog Create(
        string notificationType, Guid referenceId, string recipientEmail, string subject, string body, bool isSuccess, string? errorMessage)
    {
        return new NotificationLog
        {
            Id = Guid.NewGuid(),
            NotificationType = notificationType,
            ReferenceId = referenceId,
            RecipientEmail = recipientEmail,
            Subject = subject,
            Body = body,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            CreatedAt = DateTime.UtcNow
        };
    }
}
