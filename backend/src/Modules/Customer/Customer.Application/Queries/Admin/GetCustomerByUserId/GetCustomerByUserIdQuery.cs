using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Customer.Application.Common;

namespace Customer.Application.Queries.Admin.GetCustomerByUserId;

public record GetCustomerByUserIdQuery(Guid UserId) : IQuery<CustomerDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
