using Microsoft.EntityFrameworkCore;
using Pricing.Contracts;
using Pricing.Domain.Enums;
using Pricing.Infrastructure.Persistence;

namespace Pricing.Infrastructure.Integrations;

/// <summary>
/// Implementation of the Pricing module's outbound contract (see architecture.md §3 —
/// contracts/integrations). Read-only: other modules consume this instead of touching
/// <see cref="PricingDbContext"/> directly.
/// </summary>
public class PriceContractService(PricingDbContext dbContext) : IPriceCatalogService
{
    public async Task<PriceSummaryDto?> GetPriceAsync(Guid sellableItemId, PriceItemType sellableItemType, CancellationToken cancellationToken)
    {
        return await dbContext.Prices
            .AsNoTracking()
            .Where(p => p.SellableItemId == sellableItemId && p.SellableItemType == sellableItemType)
            .Select(p => new PriceSummaryDto(p.SellableItemId, p.SellableItemType, p.Amount))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, decimal>> GetPricesAsync(
        IReadOnlyCollection<(Guid SellableItemId, PriceItemType SellableItemType)> items,
        CancellationToken cancellationToken)
    {
        var ids = items.Select(i => i.SellableItemId).ToList();

        var prices = await dbContext.Prices
            .AsNoTracking()
            .Where(p => ids.Contains(p.SellableItemId))
            .ToListAsync(cancellationToken);

        return prices.ToDictionary(p => p.SellableItemId, p => p.Amount);
    }
}
