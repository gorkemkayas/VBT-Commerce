using Pricing.Domain.Entities;

namespace Pricing.Application.Common;

public static class CouponMapper
{
    public static CouponDto ToDto(Coupon coupon)
        => new(
            coupon.Id,
            coupon.Code,
            coupon.DiscountType,
            coupon.DiscountValue,
            coupon.MaxDiscountAmount,
            coupon.MinCartAmount,
            coupon.ScopeType,
            coupon.ScopeReferenceId,
            coupon.StartDate,
            coupon.EndDate,
            coupon.TotalUsageLimit,
            coupon.PerUserUsageLimit,
            coupon.IsActive);
}
