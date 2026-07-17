using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponInactiveException : DomainException
{
    public CouponInactiveException(string code)
        : base($"Coupon '{code}' is not active.")
    {
    }
}
