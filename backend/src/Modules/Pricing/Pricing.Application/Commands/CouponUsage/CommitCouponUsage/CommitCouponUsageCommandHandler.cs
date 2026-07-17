using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Domain.Entities;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Commands.CouponUsage.CommitCouponUsage;

public class CommitCouponUsageCommandHandler(IPricingDbContext dbContext) : IRequestHandler<CommitCouponUsageCommand, Unit>
{
    public async Task<Unit> Handle(CommitCouponUsageCommand request, CancellationToken cancellationToken)
    {
        foreach (var appliedCoupon in request.AppliedCoupons)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == appliedCoupon.Code, cancellationToken)
                ?? throw CouponNotFoundException.ForCode(appliedCoupon.Code);

            var usage = Pricing.Domain.Entities.CouponUsage.Create(
                coupon.Id, request.CustomerId, request.GuestCustomerId, request.OrderId, appliedCoupon.DiscountAmount);
            dbContext.CouponUsages.Add(usage);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
