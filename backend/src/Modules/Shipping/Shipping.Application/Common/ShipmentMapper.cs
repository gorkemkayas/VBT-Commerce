using Shipping.Domain.Entities;

namespace Shipping.Application.Common;

public static class ShipmentMapper
{
    public static ShipmentDto ToDto(Shipment shipment)
        => new(
            shipment.Id,
            shipment.OrderId,
            shipment.ShippingCompanyId,
            shipment.Status,
            shipment.TrackingNumber,
            shipment.CreatedAt,
            shipment.UpdatedAt);
}
