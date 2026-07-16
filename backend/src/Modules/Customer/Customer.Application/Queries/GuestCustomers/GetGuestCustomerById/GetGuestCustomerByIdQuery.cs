using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Customer.Application.Common;

namespace Customer.Application.Queries.GuestCustomers.GetGuestCustomerById;

public record GetGuestCustomerByIdQuery(Guid GuestCustomerId) : IQuery<GuestCustomerDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
