using Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Customer.Application.Abstractions;

public interface ICustomerDbContext
{
    DbSet<CustomerProfile> Customers { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }
    DbSet<GuestCustomer> GuestCustomers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
