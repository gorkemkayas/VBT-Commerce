using Pricing.Domain.Enums;

namespace Pricing.Contracts;

/// <summary>
/// Read-only contract exposed by the Pricing module to other modules (Cart, Order, ...) that need to
/// display or snapshot a sellable item's current price without depending on Pricing's persistence.
/// </summary>
public interface IPriceCatalogService
{
    Task<PriceSummaryDto?> GetPriceAsync(Guid sellableItemId, PriceItemType sellableItemType, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, decimal>> GetPricesAsync(
        IReadOnlyCollection<(Guid SellableItemId, PriceItemType SellableItemType)> items,
        CancellationToken cancellationToken);
}
