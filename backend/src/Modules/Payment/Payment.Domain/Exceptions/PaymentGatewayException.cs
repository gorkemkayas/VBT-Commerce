using BuildingBlocks.Domain;

namespace Payment.Domain.Exceptions;

public class PaymentGatewayException : DomainException
{
    public PaymentGatewayException(string reason)
        : base($"Payment gateway error: {reason}")
    {
    }
}
