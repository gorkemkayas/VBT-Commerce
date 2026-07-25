using Shipping.Domain.Enums;

namespace Shipping.Contracts;

public record ShipmentTrackingDto(
    Guid Id,
    ShipmentStatus Status,
    string? TrackingNumber,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyCollection<ShipmentStatusHistoryDto> History);

public record ShipmentStatusHistoryDto(ShipmentStatus Status, string? TrackingNumber, DateTime CreatedAt);
