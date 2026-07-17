using Catalog.Contracts;
using Review.Application.Integrations;
using Review.Domain.Enums;

namespace Review.Infrastructure.Integrations;

/// <summary>
/// Adapts Review's own <see cref="ICatalogIntegrationService"/> to Catalog's actual contract
/// (<see cref="IProductCatalogService"/>), so Review.Application never references Catalog directly.
/// </summary>
public class CatalogIntegrationService(IProductCatalogService productCatalogService) : ICatalogIntegrationService
{
    public async Task<bool> SellableItemExistsAsync(Guid sellableItemId, ReviewItemType sellableItemType, CancellationToken cancellationToken)
    {
        return sellableItemType switch
        {
            ReviewItemType.Product => (await productCatalogService.GetProductAsync(sellableItemId, cancellationToken))?.IsActive ?? false,
            ReviewItemType.Variant => (await productCatalogService.GetVariantAsync(sellableItemId, cancellationToken))?.IsActive ?? false,
            _ => false
        };
    }
}
