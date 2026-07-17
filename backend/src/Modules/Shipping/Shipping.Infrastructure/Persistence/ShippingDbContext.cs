using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shipping.Application.Abstractions;
using Shipping.Domain.Entities;

namespace Shipping.Infrastructure.Persistence;

public class ShippingDbContext(DbContextOptions<ShippingDbContext> options) : DbContext(options), IShippingDbContext
{
    public DbSet<ShippingCompany> ShippingCompanies => Set<ShippingCompany>();
    public DbSet<Shipment> Shipments => Set<Shipment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shipping_schema");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShippingDbContext).Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);
}
