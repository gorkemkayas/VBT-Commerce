using Pricing.Domain.Enums;

namespace Pricing.Contracts;

public record PriceCalculationLineDto(Guid SellableItemId, PriceItemType SellableItemType, int Quantity, decimal UnitPrice, decimal LineSubtotal);

public record AppliedCouponDto(string Code, decimal DiscountAmount);

public record PriceCalculationResultDto(
    IReadOnlyCollection<PriceCalculationLineDto> Lines,
    decimal Subtotal,
    IReadOnlyCollection<AppliedCouponDto> AppliedCoupons,
    decimal TotalDiscount,
    decimal TaxRate,
    decimal TaxAmount,
    decimal GrandTotal);
