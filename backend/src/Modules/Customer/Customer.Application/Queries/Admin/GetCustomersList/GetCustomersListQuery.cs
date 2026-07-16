using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Customer.Application.Common;

namespace Customer.Application.Queries.Admin.GetCustomersList;

public record GetCustomersListQuery(int PageNumber = 1, int PageSize = 20) : IQuery<PagedResult<CustomerListItemDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
