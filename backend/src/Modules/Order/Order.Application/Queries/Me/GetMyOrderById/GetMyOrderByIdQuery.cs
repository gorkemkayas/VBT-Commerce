using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Order.Application.Common;

namespace Order.Application.Queries.Me.GetMyOrderById;

public record GetMyOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
