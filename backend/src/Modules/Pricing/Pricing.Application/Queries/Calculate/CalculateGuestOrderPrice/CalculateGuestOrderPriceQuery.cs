using BuildingBlocks.Application.Messaging;
using Pricing.Contracts;

namespace Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;

public record CalculateGuestOrderPriceQuery(
    Guid GuestCustomerId,
    IReadOnlyCollection<PriceCalculationItem> Items,
    IReadOnlyCollection<string> CouponCodes) : IQuery<PriceCalculationResultDto>;
