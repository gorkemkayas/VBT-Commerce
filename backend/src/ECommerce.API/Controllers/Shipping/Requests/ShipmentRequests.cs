using Shipping.Domain.Enums;

namespace ECommerce.API.Controllers.Shipping.Requests;

public record UpdateShipmentStatusRequest(ShipmentStatus Status, string? TrackingNumber);
