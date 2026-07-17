using Cart.Domain.Enums;

namespace Cart.Contracts;

public record CartItemSummaryDto(Guid SellableItemId, CartItemType SellableItemType, int Quantity);
