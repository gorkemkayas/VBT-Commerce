using Catalog.Contracts;
using Inventory.Application.Integrations;
using Inventory.Domain.Enums;

namespace Inventory.Infrastructure.Integrations;

/// <summary>
/// Adapts Inventory's own <see cref="ICatalogIntegrationService"/> to Catalog's actual contract
/// (<see cref="IProductCatalogService"/>), so Inventory.Application never references Catalog directly.
/// </summary>
public class CatalogIntegrationService(IProductCatalogService productCatalogService) : ICatalogIntegrationService
{
    public Task<bool> SellableItemExistsAsync(Guid sellableItemId, InventoryItemType sellableItemType, CancellationToken cancellationToken)
    {
        return sellableItemType switch
        {
            InventoryItemType.Product => productCatalogService.ProductExistsAsync(sellableItemId, cancellationToken),
            InventoryItemType.Variant => productCatalogService.VariantExistsAsync(sellableItemId, cancellationToken),
            _ => Task.FromResult(false)
        };
    }
}
