using BuildingBlocks.Application.Messaging;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.Shipments.GetShipmentByOrderId;

/// <summary>
/// No IRequireRole and no controller endpoint by design — the future Order module will call this
/// in-process after verifying the requesting customer owns the order (same pattern as CreateShipmentCommand).
/// </summary>
public record GetShipmentByOrderIdQuery(Guid OrderId) : IQuery<ShipmentDto>;
