using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class CouponCodeAlreadyExistsException : DomainException
{
    public CouponCodeAlreadyExistsException(string code)
        : base($"A coupon with code '{code}' already exists.")
    {
    }
}
