namespace Notification.Application.Email;

/// <summary>
/// Abstracts the actual mail transport (Infrastructure implements this against an SMTP sandbox —
/// see Payment's IIyzicoGateway for the same Application-defines/Infrastructure-implements shape
/// used for an external, credential-bearing dependency). Never throws for a delivery failure — it
/// reports failure in the result so the caller can log it to NotificationLog instead of losing the
/// outbox message to an uncaught exception.
/// </summary>
public interface IEmailSender
{
    Task<EmailSendResult> SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken);
}

public record EmailSendResult(bool IsSuccess, string? ErrorMessage);
