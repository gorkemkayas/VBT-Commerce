using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;

namespace Pricing.Domain.Entities;

/// <summary>
/// Owns the sale price of a sellable item, keyed the same way Inventory keys its StockItem
/// (SellableItemId + SellableItemType) — Catalog deliberately holds no price, Pricing is its owner.
/// </summary>
public class Price
{
    public Guid Id { get; private set; }
    public Guid SellableItemId { get; private set; }
    public PriceItemType SellableItemType { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Price()
    {
    }

    public static Price Create(Guid sellableItemId, PriceItemType sellableItemType, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidPriceAmountException(amount);

        return new Price
        {
            Id = Guid.NewGuid(),
            SellableItemId = sellableItemId,
            SellableItemType = sellableItemType,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidPriceAmountException(amount);

        Amount = amount;
        UpdatedAt = DateTime.UtcNow;
    }
}
