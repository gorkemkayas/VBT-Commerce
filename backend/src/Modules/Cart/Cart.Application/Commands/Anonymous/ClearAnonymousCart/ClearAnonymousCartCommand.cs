using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Cart.Application.Commands.Anonymous.ClearAnonymousCart;

public record ClearAnonymousCartCommand(Guid AnonymousId) : ICommand<Unit>;
