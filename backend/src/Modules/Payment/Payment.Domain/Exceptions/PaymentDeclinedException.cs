using BuildingBlocks.Domain;

namespace Payment.Domain.Exceptions;

public class PaymentDeclinedException : DomainException
{
    public PaymentDeclinedException(string reason)
        : base($"Payment was declined: {reason}")
    {
    }
}
