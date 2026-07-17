using Shipping.Domain.Enums;

namespace Shipping.Application.Common;

public record ShipmentDto(
    Guid Id,
    Guid OrderId,
    Guid ShippingCompanyId,
    ShipmentStatus Status,
    string? TrackingNumber,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
