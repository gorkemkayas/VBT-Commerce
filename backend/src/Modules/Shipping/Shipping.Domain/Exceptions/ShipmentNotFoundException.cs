using BuildingBlocks.Domain;

namespace Shipping.Domain.Exceptions;

public class ShipmentNotFoundException : DomainException
{
    public ShipmentNotFoundException(Guid shipmentId)
        : base($"Shipment '{shipmentId}' was not found.")
    {
    }
}
