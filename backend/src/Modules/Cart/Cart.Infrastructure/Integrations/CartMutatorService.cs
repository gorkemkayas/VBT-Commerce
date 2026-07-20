using Cart.Application.Commands.Anonymous.ClearAnonymousCart;
using Cart.Application.Commands.ClearCartByUserId;
using Cart.Contracts;
using MediatR;

namespace Cart.Infrastructure.Integrations;

/// <summary>
/// Implementation of Cart's outbound write contract (<see cref="ICartMutator"/>). Delegates to the
/// same MediatR commands used internally, instead of duplicating CartOperations.ClearAsync's logic.
/// </summary>
public class CartMutatorService(ISender sender) : ICartMutator
{
    public Task ClearByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => sender.Send(new ClearCartByUserIdCommand(userId), cancellationToken);

    public Task ClearByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken)
        => sender.Send(new ClearAnonymousCartCommand(anonymousId), cancellationToken);
}
