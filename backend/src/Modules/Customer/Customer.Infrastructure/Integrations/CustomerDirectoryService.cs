using Customer.Contracts;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Integrations;

/// <summary>
/// Implementation of the Customer module's outbound contract (see architecture.md §3 —
/// contracts/integrations). Read-only: other modules consume this instead of touching
/// <see cref="CustomerDbContext"/> directly.
/// </summary>
public class CustomerDirectoryService(CustomerDbContext dbContext) : ICustomerDirectoryService
{
    public async Task<CustomerSummaryDto?> GetCustomerByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new CustomerSummaryDto(c.Id, c.UserId, c.PhoneNumber))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> CustomerExistsAsync(Guid customerId, CancellationToken cancellationToken)
        => dbContext.Customers.AsNoTracking().AnyAsync(c => c.Id == customerId, cancellationToken);

    public async Task<GuestCustomerSummaryDto?> GetGuestCustomerAsync(Guid guestCustomerId, CancellationToken cancellationToken)
    {
        return await dbContext.GuestCustomers
            .AsNoTracking()
            .Where(g => g.Id == guestCustomerId)
            .Select(g => new GuestCustomerSummaryDto(g.Id, g.FirstName, g.LastName, g.Email, g.PhoneNumber))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> GuestCustomerExistsAsync(Guid guestCustomerId, CancellationToken cancellationToken)
        => dbContext.GuestCustomers.AsNoTracking().AnyAsync(g => g.Id == guestCustomerId, cancellationToken);

    public async Task<CustomerAddressSummaryDto?> GetCustomerAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken)
    {
        return await dbContext.CustomerAddresses
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId && a.Id == addressId)
            .Select(a => new CustomerAddressSummaryDto(
                a.Id, a.RecipientName, a.PhoneNumber, a.Country, a.City, a.District, a.PostalCode, a.AddressLine1, a.AddressLine2))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
