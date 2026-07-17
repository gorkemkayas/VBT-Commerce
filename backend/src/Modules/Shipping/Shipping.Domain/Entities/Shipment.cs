using Shipping.Domain.Enums;
using Shipping.Domain.Exceptions;

namespace Shipping.Domain.Entities;

/// <summary>
/// Tracks the delivery of a single order via a chosen ShippingCompany. Created in-process by the
/// future Order module on order placement (no consumer yet — mirrors Pricing's CommitCouponUsageCommand
/// and Inventory's Reserve/Confirm/Release: infrastructure built ahead of its caller).
/// Status is updated manually by Admin (project-overview.md §4: "Delivery status is updated manually
/// by the relevant person through the system") — Delivered/Cancelled are terminal.
/// </summary>
public class Shipment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ShippingCompanyId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public string? TrackingNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Shipment()
    {
    }

    public static Shipment Create(Guid orderId, Guid shippingCompanyId)
    {
        return new Shipment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ShippingCompanyId = shippingCompanyId,
            Status = ShipmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(ShipmentStatus status, string? trackingNumber)
    {
        if (Status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled)
            throw new ShipmentAlreadyFinalizedException(Id);

        Status = status;
        if (trackingNumber is not null)
            TrackingNumber = trackingNumber;
        UpdatedAt = DateTime.UtcNow;
    }
}
