using BuildingBlocks.Domain;

namespace Payment.Domain.Exceptions;

public class PaymentNotFoundException : DomainException
{
    public PaymentNotFoundException(Guid id)
        : base($"Payment '{id}' was not found.")
    {
    }
}
