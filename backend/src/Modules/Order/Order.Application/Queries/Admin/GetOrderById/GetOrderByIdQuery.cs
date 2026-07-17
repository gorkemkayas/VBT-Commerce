using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Order.Application.Common;

namespace Order.Application.Queries.Admin.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
