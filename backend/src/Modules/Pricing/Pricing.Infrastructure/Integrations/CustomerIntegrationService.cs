using Customer.Contracts;
using Pricing.Application.Integrations;

namespace Pricing.Infrastructure.Integrations;

/// <summary>
/// Adapts Pricing's own <see cref="ICustomerIntegrationService"/> to Customer's actual contract
/// (<see cref="ICustomerDirectoryService"/>), so Pricing.Application never references Customer directly.
/// </summary>
public class CustomerIntegrationService(ICustomerDirectoryService customerDirectoryService) : ICustomerIntegrationService
{
    public Task<bool> GuestCustomerExistsAsync(Guid guestCustomerId, CancellationToken cancellationToken)
        => customerDirectoryService.GuestCustomerExistsAsync(guestCustomerId, cancellationToken);
}
