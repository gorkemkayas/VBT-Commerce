using BuildingBlocks.Domain;
using Order.Domain.Enums;

namespace Order.Domain.Exceptions;

public class OrderInvalidStatusTransitionException : DomainException
{
    public OrderInvalidStatusTransitionException(Guid orderId, OrderStatus currentStatus)
        : base($"Order '{orderId}' is in status '{currentStatus}' and cannot be transitioned further.")
    {
    }
}
