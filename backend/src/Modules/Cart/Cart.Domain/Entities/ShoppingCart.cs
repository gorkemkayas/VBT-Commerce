using Cart.Domain.Enums;
using Cart.Domain.Exceptions;

namespace Cart.Domain.Entities;

/// <summary>
/// Aggregate root. Named "ShoppingCart" rather than "Cart" to avoid a namespace/type collision with
/// the root "Cart.*" namespace tree (the same hazard fixed for Customer.Domain's aggregate root).
/// Owned by exactly one of a registered user (<see cref="UserId"/>) or an anonymous, client-generated
/// identity (<see cref="AnonymousId"/>) — never both, never neither.
/// </summary>
public class ShoppingCart
{
    private readonly List<CartItem> _items = [];

    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? AnonymousId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private ShoppingCart()
    {
    }

    public static ShoppingCart Create(Guid? userId, Guid? anonymousId)
    {
        if (userId is null == anonymousId is null)
            throw new ArgumentException("Exactly one of userId or anonymousId must be provided.");

        return new ShoppingCart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AnonymousId = anonymousId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Adds a new line, or increases the quantity of an existing line for the same sellable item.
    /// Returns the affected item and whether it is newly created (callers must explicitly track new
    /// child entities with the DbContext — EF Core does not reliably detect them as Added otherwise).
    /// </summary>
    public (CartItem Item, bool IsNew) AddItem(Guid sellableItemId, CartItemType sellableItemType, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.SellableItemId == sellableItemId && i.SellableItemType == sellableItemType);

        UpdatedAt = DateTime.UtcNow;

        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            return (existing, false);
        }

        var item = CartItem.Create(Id, sellableItemId, sellableItemType, quantity);
        _items.Add(item);
        return (item, true);
    }

    public void UpdateItemQuantity(Guid cartItemId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new CartItemNotFoundException(cartItemId);

        item.ChangeQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public CartItem RemoveItem(Guid cartItemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new CartItemNotFoundException(cartItemId);

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;

        return item;
    }

    public IReadOnlyCollection<CartItem> Clear()
    {
        var removed = _items.ToList();
        _items.Clear();
        UpdatedAt = DateTime.UtcNow;

        return removed;
    }
}
