using BuildingBlocks.Application.Messaging;
using Identity.Application.Common;
using Identity.Domain.Enums;

namespace Identity.Application.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    ClientPlatform Platform) : ICommand<AuthResult>;
