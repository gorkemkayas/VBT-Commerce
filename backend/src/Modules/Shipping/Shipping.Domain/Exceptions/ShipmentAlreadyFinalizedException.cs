using BuildingBlocks.Domain;

namespace Shipping.Domain.Exceptions;

public class ShipmentAlreadyFinalizedException : DomainException
{
    public ShipmentAlreadyFinalizedException(Guid shipmentId)
        : base($"Shipment '{shipmentId}' is already finalized and cannot be updated further.")
    {
    }
}
