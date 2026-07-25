using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Shipping.Contracts;

namespace Order.Application.Queries.Me.GetMyOrderShipment;

public record GetMyOrderShipmentQuery(Guid OrderId) : IQuery<ShipmentTrackingDto>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
