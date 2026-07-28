using BuildingBlocks.Application.Messaging;
using Pricing.Application.Common;
using Pricing.Domain.Enums;

namespace Pricing.Application.Queries.Prices.GetPrices;

public record PriceLookupItem(Guid SellableItemId, PriceItemType SellableItemType);

public record GetPricesQuery(IReadOnlyCollection<PriceLookupItem> Items) : IQuery<IReadOnlyCollection<PriceDto>>;
