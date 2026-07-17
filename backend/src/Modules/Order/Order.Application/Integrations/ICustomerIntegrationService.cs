using Customer.Contracts;

namespace Order.Application.Integrations;

public interface ICustomerIntegrationService
{
    Task<CustomerSummaryDto?> GetCustomerByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<CustomerAddressSummaryDto?> GetCustomerAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken);

    Task<bool> GuestCustomerExistsAsync(Guid guestCustomerId, CancellationToken cancellationToken);

    Task<GuestCustomerSummaryDto?> GetGuestCustomerAsync(Guid guestCustomerId, CancellationToken cancellationToken);
}
