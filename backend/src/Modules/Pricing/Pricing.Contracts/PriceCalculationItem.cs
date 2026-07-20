using Pricing.Domain.Enums;

namespace Pricing.Contracts;

/// <summary>
/// A single line the caller wants priced. Pricing never reads a cart itself — the caller (the
/// frontend, or Order during checkout) enumerates the items directly.
/// </summary>
public record PriceCalculationItem(Guid SellableItemId, PriceItemType SellableItemType, int Quantity);
