using BuildingBlocks.Application.Security;
using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Me.UpdateMyCartItemQuantity;

public class UpdateMyCartItemQuantityCommandHandler(CartOperations cartOperations, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateMyCartItemQuantityCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMyCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForUser(currentUserService.UserId!.Value);

        await cartOperations.UpdateItemQuantityAsync(owner, request.CartItemId, request.Quantity, cancellationToken);

        return Unit.Value;
    }
}
