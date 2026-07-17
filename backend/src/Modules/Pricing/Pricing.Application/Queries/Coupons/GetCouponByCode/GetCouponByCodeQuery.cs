using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Coupons.GetCouponByCode;

public record GetCouponByCodeQuery(string Code) : IQuery<CouponDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
