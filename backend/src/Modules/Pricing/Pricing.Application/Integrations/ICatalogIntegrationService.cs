using Pricing.Domain.Enums;

namespace Pricing.Application.Integrations;

/// <summary>
/// Pricing's own view of what it needs from the Catalog module (architecture.md §3). Used to validate
/// a sellable item exists before pricing it, and to resolve category scope for Category-scoped coupons
/// (for a Variant, the parent Product's CategoryId). Implemented in Pricing.Infrastructure against
/// Catalog's IProductCatalogService.
/// </summary>
public interface ICatalogIntegrationService
{
    Task<bool> SellableItemExistsAsync(Guid sellableItemId, PriceItemType sellableItemType, CancellationToken cancellationToken);

    Task<Guid?> GetCategoryIdAsync(Guid sellableItemId, PriceItemType sellableItemType, CancellationToken cancellationToken);
}
