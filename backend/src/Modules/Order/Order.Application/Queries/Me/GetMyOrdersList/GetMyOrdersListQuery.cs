using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Order.Application.Common;

namespace Order.Application.Queries.Me.GetMyOrdersList;

public record GetMyOrdersListQuery(int PageNumber = 1, int PageSize = 20) : IQuery<PagedResult<OrderDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
