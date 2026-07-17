using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Coupons.GetCouponsList;

public record GetCouponsListQuery(int PageNumber = 1, int PageSize = 20) : IQuery<PagedResult<CouponDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
