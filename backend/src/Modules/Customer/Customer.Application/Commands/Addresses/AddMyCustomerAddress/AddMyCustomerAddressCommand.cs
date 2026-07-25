using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Customer.Application.Commands.Addresses.AddMyCustomerAddress;

public record AddMyCustomerAddressCommand(
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Country,
    string City,
    string District,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2,
    bool IsDefault,
    bool IsShippingAddress,
    bool IsBillingAddress) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
