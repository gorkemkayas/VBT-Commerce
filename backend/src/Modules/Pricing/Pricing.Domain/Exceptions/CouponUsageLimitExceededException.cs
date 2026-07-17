using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponUsageLimitExceededException : DomainException
{
    private CouponUsageLimitExceededException(string message) : base(message)
    {
    }

    public static CouponUsageLimitExceededException ForTotalLimit(string code)
        => new($"Coupon '{code}' has reached its total usage limit.");

    public static CouponUsageLimitExceededException ForPerUserLimit(string code)
        => new($"Coupon '{code}' has already reached its usage limit for this customer.");
}
