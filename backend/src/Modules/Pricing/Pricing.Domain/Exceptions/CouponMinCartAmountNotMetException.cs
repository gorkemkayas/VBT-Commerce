using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponMinCartAmountNotMetException : DomainException
{
    public CouponMinCartAmountNotMetException(string code, decimal minCartAmount)
        : base($"Coupon '{code}' requires a minimum cart amount of '{minCartAmount}'.")
    {
    }
}
