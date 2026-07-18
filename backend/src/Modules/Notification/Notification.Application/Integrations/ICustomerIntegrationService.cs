namespace Notification.Application.Integrations;

/// <summary>
/// Notification's own view of what it needs from the Customer module: a guest customer's email.
/// Implemented in Notification.Infrastructure against Customer's ICustomerDirectoryService, so
/// Notification.Application never depends on Customer directly.
/// </summary>
public interface ICustomerIntegrationService
{
    Task<string?> GetGuestCustomerEmailAsync(Guid guestCustomerId, CancellationToken cancellationToken);
}
