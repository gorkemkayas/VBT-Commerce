using Pricing.Contracts;

namespace ECommerce.API.Controllers.Pricing;

public record CalculateMyOrderPriceRequest(IReadOnlyCollection<PriceCalculationItem> Items, IReadOnlyCollection<string> CouponCodes);

public record CalculateGuestOrderPriceRequest(
    Guid GuestCustomerId, IReadOnlyCollection<PriceCalculationItem> Items, IReadOnlyCollection<string> CouponCodes);
