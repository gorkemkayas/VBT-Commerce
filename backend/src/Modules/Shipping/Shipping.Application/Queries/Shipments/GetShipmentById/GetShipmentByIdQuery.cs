using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.Shipments.GetShipmentById;

public record GetShipmentByIdQuery(Guid ShipmentId) : IQuery<ShipmentDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
