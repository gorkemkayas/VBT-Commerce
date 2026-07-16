using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Cart.Application.Commands.Anonymous.UpdateAnonymousCartItemQuantity;

public record UpdateAnonymousCartItemQuantityCommand(Guid AnonymousId, Guid CartItemId, int Quantity) : ICommand<Unit>;
