using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Order.Application.Common;
using Order.Domain.Enums;

namespace Order.Application.Queries.Admin.GetOrdersList;

public record GetOrdersListQuery(OrderStatus? Status = null, int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<OrderDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
