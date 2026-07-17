using BuildingBlocks.Domain;

namespace Payment.Domain.Exceptions;

public class PaymentAlreadyRefundedException : DomainException
{
    public PaymentAlreadyRefundedException(Guid paymentId)
        : base($"Payment '{paymentId}' has already been refunded.")
    {
    }
}
