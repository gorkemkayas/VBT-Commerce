using BuildingBlocks.Application.Messaging;
using Cart.Domain.Enums;

namespace Cart.Application.Commands.Anonymous.AddItemToAnonymousCart;

// No IRequireRole — publicly accessible. AnonymousId is client-supplied because there is no JWT to
// resolve it from; anonymous carts have no security boundary to protect (only a correlation key).
public record AddItemToAnonymousCartCommand(Guid AnonymousId, Guid SellableItemId, CartItemType SellableItemType, int Quantity)
    : ICommand<Guid>;
