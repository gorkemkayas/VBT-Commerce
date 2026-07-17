using BuildingBlocks.Domain;

namespace Shipping.Domain.Exceptions;

public class InvalidShippingFeeException : DomainException
{
    public InvalidShippingFeeException(decimal fee)
        : base($"Shipping fee '{fee}' is invalid. Fee must be greater than zero.")
    {
    }
}
