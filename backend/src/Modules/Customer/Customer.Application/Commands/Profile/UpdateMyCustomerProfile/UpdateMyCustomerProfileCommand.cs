using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Customer.Application.Commands.Profile.UpdateMyCustomerProfile;

public record UpdateMyCustomerProfileCommand(string? PhoneNumber, DateOnly? DateOfBirth) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
