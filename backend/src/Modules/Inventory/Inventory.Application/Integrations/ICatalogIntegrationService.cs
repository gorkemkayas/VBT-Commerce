using Inventory.Domain.Enums;

namespace Inventory.Application.Integrations;

/// <summary>
/// Inventory's own view of what it needs from the Catalog module (architecture.md §3 — integrations/
/// consume another module's contract). Implemented in Inventory.Infrastructure against
/// Catalog.Contracts.IProductCatalogService, so Inventory.Application never depends on Catalog directly.
/// </summary>
public interface ICatalogIntegrationService
{
    Task<bool> SellableItemExistsAsync(Guid sellableItemId, InventoryItemType sellableItemType, CancellationToken cancellationToken);
}
