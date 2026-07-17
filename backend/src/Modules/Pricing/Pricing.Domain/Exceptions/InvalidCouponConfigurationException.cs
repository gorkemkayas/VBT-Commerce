using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class InvalidCouponConfigurationException : DomainException
{
    public InvalidCouponConfigurationException(string message) : base(message)
    {
    }
}
