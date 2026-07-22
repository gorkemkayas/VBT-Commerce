using Cart.Domain.Enums;

namespace ECommerce.API.Controllers.Carts.Requests;

public record AddCartItemRequest(Guid SellableItemId, CartItemType SellableItemType, int Quantity);

public record UpdateCartItemQuantityRequest(int Quantity);
