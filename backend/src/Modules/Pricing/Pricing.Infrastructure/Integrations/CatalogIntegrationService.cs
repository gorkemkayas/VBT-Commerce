using Catalog.Contracts;
using Pricing.Application.Integrations;
using Pricing.Domain.Enums;

namespace Pricing.Infrastructure.Integrations;

/// <summary>
/// Adapts Pricing's own <see cref="ICatalogIntegrationService"/> to Catalog's actual contract
/// (<see cref="IProductCatalogService"/>), so Pricing.Application never references Catalog directly.
/// </summary>
public class CatalogIntegrationService(IProductCatalogService productCatalogService) : ICatalogIntegrationService
{
    public async Task<bool> SellableItemExistsAsync(Guid sellableItemId, PriceItemType sellableItemType, CancellationToken cancellationToken)
    {
        return sellableItemType switch
        {
            PriceItemType.Product => await productCatalogService.ProductExistsAsync(sellableItemId, cancellationToken),
            PriceItemType.Variant => await productCatalogService.VariantExistsAsync(sellableItemId, cancellationToken),
            _ => false
        };
    }

    public async Task<Guid?> GetCategoryIdAsync(Guid sellableItemId, PriceItemType sellableItemType, CancellationToken cancellationToken)
    {
        if (sellableItemType == PriceItemType.Product)
        {
            var product = await productCatalogService.GetProductAsync(sellableItemId, cancellationToken);
            return product?.CategoryId;
        }

        var variant = await productCatalogService.GetVariantAsync(sellableItemId, cancellationToken);
        if (variant is null)
            return null;

        var parentProduct = await productCatalogService.GetProductAsync(variant.ProductId, cancellationToken);
        return parentProduct?.CategoryId;
    }
}
