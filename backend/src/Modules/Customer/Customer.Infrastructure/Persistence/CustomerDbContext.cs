using Customer.Application.Abstractions;
using Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Customer.Infrastructure.Persistence;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options), ICustomerDbContext
{
    public DbSet<CustomerProfile> Customers => Set<CustomerProfile>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<GuestCustomer> GuestCustomers => Set<GuestCustomer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customer_schema");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);
}
