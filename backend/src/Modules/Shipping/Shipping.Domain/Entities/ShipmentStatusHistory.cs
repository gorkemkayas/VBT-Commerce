using Shipping.Domain.Enums;

namespace Shipping.Domain.Entities;

/// <summary>
/// One entry in a Shipment's status timeline ("story") — an immutable snapshot recorded every time
/// the shipment's status changes, including its initial creation as Pending.
/// </summary>
public class ShipmentStatusHistory
{
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public string? TrackingNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ShipmentStatusHistory() { }

    internal static ShipmentStatusHistory Create(Guid shipmentId, ShipmentStatus status, string? trackingNumber)
    {
        return new ShipmentStatusHistory
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipmentId,
            Status = status,
            TrackingNumber = trackingNumber,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
