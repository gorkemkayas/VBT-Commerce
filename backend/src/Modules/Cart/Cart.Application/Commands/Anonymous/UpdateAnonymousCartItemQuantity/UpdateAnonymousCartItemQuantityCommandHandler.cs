using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Anonymous.UpdateAnonymousCartItemQuantity;

public class UpdateAnonymousCartItemQuantityCommandHandler(CartOperations cartOperations)
    : IRequestHandler<UpdateAnonymousCartItemQuantityCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAnonymousCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForAnonymous(request.AnonymousId);

        await cartOperations.UpdateItemQuantityAsync(owner, request.CartItemId, request.Quantity, cancellationToken);

        return Unit.Value;
    }
}
