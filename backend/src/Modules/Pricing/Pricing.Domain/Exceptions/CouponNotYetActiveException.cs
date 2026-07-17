using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponNotYetActiveException : DomainException
{
    public CouponNotYetActiveException(string code)
        : base($"Coupon '{code}' is not yet valid.")
    {
    }
}
