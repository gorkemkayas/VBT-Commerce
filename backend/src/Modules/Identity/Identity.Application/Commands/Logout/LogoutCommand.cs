using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Identity.Application.Commands.Logout;

public record LogoutCommand(string RefreshToken) : ICommand<Unit>;
