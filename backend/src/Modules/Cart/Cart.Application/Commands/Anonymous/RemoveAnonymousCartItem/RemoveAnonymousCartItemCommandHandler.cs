using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Anonymous.RemoveAnonymousCartItem;

public class RemoveAnonymousCartItemCommandHandler(CartOperations cartOperations)
    : IRequestHandler<RemoveAnonymousCartItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveAnonymousCartItemCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForAnonymous(request.AnonymousId);

        await cartOperations.RemoveItemAsync(owner, request.CartItemId, cancellationToken);

        return Unit.Value;
    }
}
