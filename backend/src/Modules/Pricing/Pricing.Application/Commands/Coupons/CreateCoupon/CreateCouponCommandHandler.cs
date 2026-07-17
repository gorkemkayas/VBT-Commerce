using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Domain.Entities;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Commands.Coupons.CreateCoupon;

public class CreateCouponCommandHandler(IPricingDbContext dbContext) : IRequestHandler<CreateCouponCommand, Guid>
{
    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await dbContext.Coupons.AnyAsync(c => c.Code == request.Code, cancellationToken);
        if (codeExists)
            throw new CouponCodeAlreadyExistsException(request.Code);

        var coupon = Coupon.Create(
            request.Code,
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

        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(cancellationToken);

        return coupon.Id;
    }
}
