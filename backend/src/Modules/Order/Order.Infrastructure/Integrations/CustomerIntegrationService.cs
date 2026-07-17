using Customer.Contracts;
using Order.Application.Integrations;

namespace Order.Infrastructure.Integrations;

public class CustomerIntegrationService(ICustomerDirectoryService customerDirectoryService) : ICustomerIntegrationService
{
    public Task<CustomerSummaryDto?> GetCustomerByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => customerDirectoryService.GetCustomerByUserIdAsync(userId, cancellationToken);

    public Task<CustomerAddressSummaryDto?> GetCustomerAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken)
        => customerDirectoryService.GetCustomerAddressAsync(customerId, addressId, cancellationToken);

    public Task<bool> GuestCustomerExistsAsync(Guid guestCustomerId, CancellationToken cancellationToken)
        => customerDirectoryService.GuestCustomerExistsAsync(guestCustomerId, cancellationToken);

    public Task<GuestCustomerSummaryDto?> GetGuestCustomerAsync(Guid guestCustomerId, CancellationToken cancellationToken)
        => customerDirectoryService.GetGuestCustomerAsync(guestCustomerId, cancellationToken);
}
