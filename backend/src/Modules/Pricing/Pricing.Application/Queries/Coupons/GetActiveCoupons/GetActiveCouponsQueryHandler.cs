using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Coupons.GetActiveCoupons;

public class GetActiveCouponsQueryHandler(IPricingDbContext dbContext)
    : IRequestHandler<GetActiveCouponsQuery, IReadOnlyList<CouponDto>>
{
    public async Task<IReadOnlyList<CouponDto>> Handle(GetActiveCouponsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var coupons = await dbContext.Coupons
            .AsNoTracking()
            .Where(c => c.IsActive && c.StartDate <= now && c.EndDate >= now)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return coupons.Select(CouponMapper.ToDto).ToList();
    }
}
