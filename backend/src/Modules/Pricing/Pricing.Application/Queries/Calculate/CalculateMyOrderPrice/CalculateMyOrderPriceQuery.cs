using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;

public record CalculateMyOrderPriceQuery(
    IReadOnlyCollection<PriceCalculationItem> Items,
    IReadOnlyCollection<string> CouponCodes) : IQuery<PriceCalculationResultDto>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
