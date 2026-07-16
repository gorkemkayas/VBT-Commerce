namespace Catalog.Contracts;

/// <summary>
/// Read-only contract exposed by the Catalog module to other modules (Inventory, Pricing, Cart,
/// Order, Review, ...) that need to validate or display product/variant identity without depending
/// on Catalog's internal persistence.
/// </summary>
public interface IProductCatalogService
{
    Task<ProductSummaryDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken);

    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken);

    Task<ProductVariantSummaryDto?> GetVariantAsync(Guid variantId, CancellationToken cancellationToken);

    Task<bool> VariantExistsAsync(Guid variantId, CancellationToken cancellationToken);
}
