using Pricing.Domain.Enums;

namespace Pricing.Application.Common;

/// <summary>
/// A single line the caller wants priced. Pricing never reads a cart itself — the caller (today the
/// frontend, later Order) enumerates the items directly.
/// </summary>
public record PriceCalculationItem(Guid SellableItemId, PriceItemType SellableItemType, int Quantity);
