using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Customer.Application.Commands.Addresses.SetDefaultMyCustomerAddress;

public record SetDefaultMyCustomerAddressCommand(Guid AddressId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
