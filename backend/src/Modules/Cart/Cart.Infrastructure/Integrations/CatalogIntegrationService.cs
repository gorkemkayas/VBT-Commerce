using Catalog.Contracts;
using Cart.Application.Integrations;
using Cart.Domain.Enums;

namespace Cart.Infrastructure.Integrations;

/// <summary>
/// Adapts Cart's own <see cref="ICatalogIntegrationService"/> to Catalog's actual contract
/// (<see cref="IProductCatalogService"/>), so Cart.Application never references Catalog directly.
/// A sellable item only counts as usable in a cart if it exists AND is active.
/// </summary>
public class CatalogIntegrationService(IProductCatalogService productCatalogService) : ICatalogIntegrationService
{
    public async Task<bool> SellableItemExistsAsync(Guid sellableItemId, CartItemType sellableItemType, CancellationToken cancellationToken)
    {
        return sellableItemType switch
        {
            CartItemType.Product => (await productCatalogService.GetProductAsync(sellableItemId, cancellationToken))?.IsActive ?? false,
            CartItemType.Variant => (await productCatalogService.GetVariantAsync(sellableItemId, cancellationToken))?.IsActive ?? false,
            _ => false
        };
    }
}
