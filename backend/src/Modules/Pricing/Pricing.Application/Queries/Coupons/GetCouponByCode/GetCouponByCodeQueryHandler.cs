using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Common;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Queries.Coupons.GetCouponByCode;

public class GetCouponByCodeQueryHandler(IPricingDbContext dbContext) : IRequestHandler<GetCouponByCodeQuery, CouponDto>
{
    public async Task<CouponDto> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken)
            ?? throw CouponNotFoundException.ForCode(request.Code);

        return CouponMapper.ToDto(coupon);
    }
}
