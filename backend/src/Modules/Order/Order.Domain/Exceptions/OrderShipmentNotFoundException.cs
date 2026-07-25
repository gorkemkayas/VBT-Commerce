using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderShipmentNotFoundException : DomainException
{
    public OrderShipmentNotFoundException(Guid orderId)
        : base($"No shipment was found for order '{orderId}'.")
    {
    }
}
