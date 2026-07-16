using Cart.Domain.Enums;

namespace Cart.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid SellableItemId { get; private set; }
    public CartItemType SellableItemType { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CartItem()
    {
    }

    internal static CartItem Create(Guid cartId, Guid sellableItemId, CartItemType sellableItemType, int quantity)
    {
        return new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            SellableItemId = sellableItemId,
            SellableItemType = sellableItemType,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void IncreaseQuantity(int delta)
    {
        if (delta <= 0)
            throw new ArgumentOutOfRangeException(nameof(delta), "Delta must be greater than zero.");

        Quantity += delta;
        UpdatedAt = DateTime.UtcNow;
    }
}
