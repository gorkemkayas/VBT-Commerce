using Pricing.Domain.Enums;

namespace Pricing.Application.Common;

public record PriceDto(Guid Id, Guid SellableItemId, PriceItemType SellableItemType, decimal Amount);
