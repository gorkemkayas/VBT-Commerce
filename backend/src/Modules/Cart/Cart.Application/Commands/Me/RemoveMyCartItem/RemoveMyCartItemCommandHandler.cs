using BuildingBlocks.Application.Security;
using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Me.RemoveMyCartItem;

public class RemoveMyCartItemCommandHandler(CartOperations cartOperations, ICurrentUserService currentUserService)
    : IRequestHandler<RemoveMyCartItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveMyCartItemCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForUser(currentUserService.UserId!.Value);

        await cartOperations.RemoveItemAsync(owner, request.CartItemId, cancellationToken);

        return Unit.Value;
    }
}
