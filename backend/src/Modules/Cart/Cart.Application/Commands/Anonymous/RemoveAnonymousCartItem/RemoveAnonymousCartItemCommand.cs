using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Cart.Application.Commands.Anonymous.RemoveAnonymousCartItem;

public record RemoveAnonymousCartItemCommand(Guid AnonymousId, Guid CartItemId) : ICommand<Unit>;
