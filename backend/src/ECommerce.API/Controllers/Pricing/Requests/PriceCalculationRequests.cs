using Pricing.Contracts;

namespace ECommerce.API.Controllers.Pricing.Requests;

public record CalculateMyOrderPriceRequest(IReadOnlyCollection<PriceCalculationItem> Items, IReadOnlyCollection<string> CouponCodes);

public record CalculateGuestOrderPriceRequest(
    Guid GuestCustomerId, IReadOnlyCollection<PriceCalculationItem> Items, IReadOnlyCollection<string> CouponCodes);
