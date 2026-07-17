using MediatR;
using Pricing.Application.Abstractions;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Commands.Coupons.UpdateCoupon;

public class UpdateCouponCommandHandler(IPricingDbContext dbContext) : IRequestHandler<UpdateCouponCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await dbContext.Coupons.FindAsync([request.CouponId], cancellationToken)
            ?? throw CouponNotFoundException.ForId(request.CouponId);

        coupon.Update(
            request.DiscountType,
            request.DiscountValue,
            request.MaxDiscountAmount,
            request.MinCartAmount,
            request.ScopeType,
            request.ScopeReferenceId,
            request.StartDate,
            request.EndDate,
            request.TotalUsageLimit,
            request.PerUserUsageLimit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
