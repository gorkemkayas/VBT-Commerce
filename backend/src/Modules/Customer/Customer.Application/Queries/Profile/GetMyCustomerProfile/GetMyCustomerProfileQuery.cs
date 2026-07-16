using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Customer.Application.Common;

namespace Customer.Application.Queries.Profile.GetMyCustomerProfile;

public record GetMyCustomerProfileQuery : IQuery<CustomerDto>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
