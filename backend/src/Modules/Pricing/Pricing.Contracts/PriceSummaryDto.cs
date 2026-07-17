using Pricing.Domain.Enums;

namespace Pricing.Contracts;

public record PriceSummaryDto(Guid SellableItemId, PriceItemType SellableItemType, decimal Amount);
