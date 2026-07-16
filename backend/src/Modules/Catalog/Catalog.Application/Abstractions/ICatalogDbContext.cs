using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.Application.Abstractions;

public interface ICatalogDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductAttribute> ProductAttributes { get; }
    DbSet<ProductVariantAttribute> ProductVariantAttributes { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductVariantOptionValue> ProductVariantOptionValues { get; }
    DbSet<ProductImage> ProductImages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
