using MediatR;
using Pricing.Application.Abstractions;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Commands.Coupons.DeactivateCoupon;

public class DeactivateCouponCommandHandler(IPricingDbContext dbContext) : IRequestHandler<DeactivateCouponCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await dbContext.Coupons.FindAsync([request.CouponId], cancellationToken)
            ?? throw CouponNotFoundException.ForId(request.CouponId);

        coupon.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
