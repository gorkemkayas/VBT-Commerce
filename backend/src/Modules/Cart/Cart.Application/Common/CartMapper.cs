using Cart.Domain.Entities;

namespace Cart.Application.Common;

public static class CartMapper
{
    public static CartDto ToDto(ShoppingCart cart)
    {
        return new CartDto(
            cart.Id,
            cart.UserId,
            cart.AnonymousId,
            cart.Items
                .Select(i => new CartItemDto(i.Id, i.SellableItemId, i.SellableItemType, i.Quantity))
                .ToList());
    }
}
