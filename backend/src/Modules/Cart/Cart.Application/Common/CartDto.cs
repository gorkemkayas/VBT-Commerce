using Cart.Domain.Enums;

namespace Cart.Application.Common;

public record CartItemDto(Guid Id, Guid SellableItemId, CartItemType SellableItemType, int Quantity);

public record CartDto(Guid Id, Guid? UserId, Guid? AnonymousId, IReadOnlyCollection<CartItemDto> Items);
