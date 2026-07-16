using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Anonymous.AddItemToAnonymousCart;

public class AddItemToAnonymousCartCommandHandler(CartOperations cartOperations)
    : IRequestHandler<AddItemToAnonymousCartCommand, Guid>
{
    public async Task<Guid> Handle(AddItemToAnonymousCartCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForAnonymous(request.AnonymousId);

        var item = await cartOperations.AddItemAsync(
            owner, request.SellableItemId, request.SellableItemType, request.Quantity, cancellationToken);

        return item.Id;
    }
}
