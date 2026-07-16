using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Customer.Application.Commands.Profile.CreateMyCustomerProfile;

public record CreateMyCustomerProfileCommand(string? PhoneNumber, DateOnly? DateOfBirth) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
