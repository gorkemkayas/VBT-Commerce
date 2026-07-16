using Cart.Domain.Enums;

namespace Cart.Application.Integrations;

/// <summary>
/// Cart's own view of what it needs from the Catalog module (architecture.md §3 — integrations/
/// consume another module's contract). Implemented in Cart.Infrastructure against Catalog's
/// IProductCatalogService, so Cart.Application never depends on Catalog directly.
/// </summary>
public interface ICatalogIntegrationService
{
    Task<bool> SellableItemExistsAsync(Guid sellableItemId, CartItemType sellableItemType, CancellationToken cancellationToken);
}
