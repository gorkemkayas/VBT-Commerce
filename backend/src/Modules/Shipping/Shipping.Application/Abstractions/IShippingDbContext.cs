using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shipping.Domain.Entities;

namespace Shipping.Application.Abstractions;

public interface IShippingDbContext
{
    DbSet<ShippingCompany> ShippingCompanies { get; }
    DbSet<Shipment> Shipments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
