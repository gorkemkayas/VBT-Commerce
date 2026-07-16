namespace Customer.Contracts;

/// <summary>
/// Read-only contract exposed by the Customer module to other modules (Cart, Order, Review, ...)
/// that need to validate or display a customer/guest-customer reference without depending on
/// Customer's internal persistence.
/// </summary>
public interface ICustomerDirectoryService
{
    Task<CustomerSummaryDto?> GetCustomerByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> CustomerExistsAsync(Guid customerId, CancellationToken cancellationToken);

    Task<GuestCustomerSummaryDto?> GetGuestCustomerAsync(Guid guestCustomerId, CancellationToken cancellationToken);

    Task<bool> GuestCustomerExistsAsync(Guid guestCustomerId, CancellationToken cancellationToken);
}
