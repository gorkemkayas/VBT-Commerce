using Customer.Contracts;
using Notification.Application.Integrations;

namespace Notification.Infrastructure.Integrations;

/// <summary>
/// Adapts Notification's own ICustomerIntegrationService to Customer's actual contract
/// (ICustomerDirectoryService), so Notification.Application never references Customer directly.
/// </summary>
public class CustomerIntegrationService(ICustomerDirectoryService customerDirectoryService) : ICustomerIntegrationService
{
    public async Task<string?> GetGuestCustomerEmailAsync(Guid guestCustomerId, CancellationToken cancellationToken)
    {
        var guestCustomer = await customerDirectoryService.GetGuestCustomerAsync(guestCustomerId, cancellationToken);
        return guestCustomer?.Email;
    }
}
