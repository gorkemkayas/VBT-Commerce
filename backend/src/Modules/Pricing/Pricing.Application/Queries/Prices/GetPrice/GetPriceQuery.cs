using BuildingBlocks.Application.Messaging;
using Pricing.Application.Common;
using Pricing.Domain.Enums;

namespace Pricing.Application.Queries.Prices.GetPrice;

public record GetPriceQuery(Guid SellableItemId, PriceItemType SellableItemType) : IQuery<PriceDto>;
