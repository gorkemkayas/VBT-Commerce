using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Prices.GetPrices;

public class GetPricesQueryHandler(IPricingDbContext dbContext) : IRequestHandler<GetPricesQuery, IReadOnlyCollection<PriceDto>>
{
    public async Task<IReadOnlyCollection<PriceDto>> Handle(GetPricesQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Items.Select(i => i.SellableItemId).ToList();

        var prices = await dbContext.Prices
            .AsNoTracking()
            .Where(p => ids.Contains(p.SellableItemId))
            .ToListAsync(cancellationToken);

        // SellableItemId is unique per the composite unique index, but matching the requested (Id, Type)
        // pairs explicitly means a mismatched type in the request can't leak a different item's price.
        var requested = request.Items.ToHashSet();
        return prices
            .Where(p => requested.Contains(new PriceLookupItem(p.SellableItemId, p.SellableItemType)))
            .Select(PriceMapper.ToDto)
            .ToList();
    }
}
