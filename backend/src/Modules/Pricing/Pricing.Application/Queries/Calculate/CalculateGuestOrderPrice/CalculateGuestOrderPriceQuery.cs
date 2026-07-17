using BuildingBlocks.Application.Messaging;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;

public record CalculateGuestOrderPriceQuery(
    Guid GuestCustomerId,
    IReadOnlyCollection<PriceCalculationItem> Items,
    IReadOnlyCollection<string> CouponCodes) : IQuery<PriceCalculationResultDto>;
