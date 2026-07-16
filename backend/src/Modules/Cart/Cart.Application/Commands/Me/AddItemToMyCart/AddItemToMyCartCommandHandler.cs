using BuildingBlocks.Application.Security;
using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Me.AddItemToMyCart;

public class AddItemToMyCartCommandHandler(CartOperations cartOperations, ICurrentUserService currentUserService)
    : IRequestHandler<AddItemToMyCartCommand, Guid>
{
    public async Task<Guid> Handle(AddItemToMyCartCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForUser(currentUserService.UserId!.Value);

        var item = await cartOperations.AddItemAsync(
            owner, request.SellableItemId, request.SellableItemType, request.Quantity, cancellationToken);

        return item.Id;
    }
}
