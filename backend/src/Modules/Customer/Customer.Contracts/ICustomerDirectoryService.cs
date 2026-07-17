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

    /// <summary>
    /// Snapshot of a registered customer's address for a future consumer (e.g. Order at checkout).
    /// Scoped by <paramref name="customerId"/> as defense-in-depth — the caller is expected to have
    /// already verified ownership, but this guards against a mismatched address/customer pair.
    /// Guest customers have no stored addresses (checkout supplies theirs inline), so there is no
    /// guest equivalent.
    /// </summary>
    Task<CustomerAddressSummaryDto?> GetCustomerAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken);
}
