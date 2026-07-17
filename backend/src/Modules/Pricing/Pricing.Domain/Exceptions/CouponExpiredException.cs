using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponExpiredException : DomainException
{
    public CouponExpiredException(string code)
        : base($"Coupon '{code}' has expired.")
    {
    }
}
