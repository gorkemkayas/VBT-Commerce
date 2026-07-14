using BuildingBlocks.Application.Messaging;
using Identity.Application.Common;
using Identity.Domain.Enums;

namespace Identity.Application.Commands.Login;

public record LoginCommand(string Email, string Password, ClientPlatform Platform) : ICommand<AuthResult>;