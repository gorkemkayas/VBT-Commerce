using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Coupons.GetCouponsList;

public class GetCouponsListQueryHandler(IPricingDbContext dbContext)
    : IRequestHandler<GetCouponsListQuery, PagedResult<CouponDto>>
{
    public async Task<PagedResult<CouponDto>> Handle(GetCouponsListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Coupons.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CouponDto>(items.Select(CouponMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
