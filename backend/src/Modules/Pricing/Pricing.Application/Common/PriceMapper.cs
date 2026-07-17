using Pricing.Domain.Entities;

namespace Pricing.Application.Common;

public static class PriceMapper
{
    public static PriceDto ToDto(Price price)
        => new(price.Id, price.SellableItemId, price.SellableItemType, price.Amount);
}
