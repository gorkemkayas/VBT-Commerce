using Catalog.Contracts;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Integrations;

/// <summary>
/// Implementation of the Catalog module's outbound contract (see architecture.md §3 — contracts/integrations).
/// Read-only: other modules consume this instead of touching <see cref="CatalogDbContext"/> directly.
/// </summary>
public class ProductCatalogService(CatalogDbContext dbContext) : IProductCatalogService
{
    public async Task<ProductSummaryDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => new ProductSummaryDto(p.Id, p.Name, p.Slug, p.ProductType, p.IsActive, p.CategoryId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken)
        => dbContext.Products.AsNoTracking().AnyAsync(p => p.Id == productId, cancellationToken);

    public async Task<ProductVariantSummaryDto?> GetVariantAsync(Guid variantId, CancellationToken cancellationToken)
    {
        return await dbContext.ProductVariants
            .AsNoTracking()
            .Where(v => v.Id == variantId)
            .Select(v => new ProductVariantSummaryDto(v.Id, v.ProductId, v.Sku, v.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> VariantExistsAsync(Guid variantId, CancellationToken cancellationToken)
        => dbContext.ProductVariants.AsNoTracking().AnyAsync(v => v.Id == variantId, cancellationToken);
}
