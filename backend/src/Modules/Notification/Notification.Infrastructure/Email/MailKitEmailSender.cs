using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Application.Email;
using Notification.Infrastructure.Options;

namespace Notification.Infrastructure.Email;

/// <summary>
/// Real SMTP delivery via MailKit — pointed at a sandbox (Mailtrap) for this project, exactly like
/// Payment's IyzicoGateway points at iyzico's sandbox: real protocol, test credentials, no risk of
/// mailing an actual customer. Never throws — SMTP/connection failures are caught and reported
/// through EmailSendResult so the caller can log them to NotificationLog instead of losing the
/// outbox message to an uncaught exception.
/// </summary>
public class MailKitEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public async Task<EmailSendResult> SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        try
        {
            using var client = new SmtpClient();

            var secureSocketOptions = _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
            await client.ConnectAsync(_options.Host, _options.Port, secureSocketOptions, cancellationToken);
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return new EmailSendResult(true, null);
        }
        catch (Exception exception)
        {
            return new EmailSendResult(false, exception.Message);
        }
    }
}
