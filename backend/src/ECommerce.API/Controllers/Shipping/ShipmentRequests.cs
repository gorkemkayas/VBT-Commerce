using Shipping.Domain.Enums;

namespace ECommerce.API.Controllers.Shipping;

public record UpdateShipmentStatusRequest(ShipmentStatus Status, string? TrackingNumber);
