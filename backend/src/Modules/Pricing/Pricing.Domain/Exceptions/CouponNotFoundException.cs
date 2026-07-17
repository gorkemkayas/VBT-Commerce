using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponNotFoundException : DomainException
{
    private CouponNotFoundException(string message) : base(message)
    {
    }

    public static CouponNotFoundException ForId(Guid couponId)
        => new($"Coupon '{couponId}' was not found.");

    public static CouponNotFoundException ForCode(string code)
        => new($"Coupon with code '{code}' was not found.");
}
