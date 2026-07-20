using BuildingBlocks.Application.Messaging;
using Pricing.Contracts;

namespace Pricing.Application.Queries.Calculate.CalculateCustomerOrderPrice;

/// <summary>
/// No IRequireRole and no controller endpoint by design — meant to be called in-process by the
/// Order module's checkout saga for a registered customer, mirroring CalculateGuestOrderPriceQuery's
/// explicit-id shape. CalculateMyOrderPriceQuery stays as the role-gated, ICurrentUserService-driven
/// HTTP-facing variant used by PriceCalculationController.
/// </summary>
public record CalculateCustomerOrderPriceQuery(
    Guid CustomerId,
    IReadOnlyCollection<PriceCalculationItem> Items,
    IReadOnlyCollection<string> CouponCodes) : IQuery<PriceCalculationResultDto>;
