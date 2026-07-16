using BuildingBlocks.Application.Security;
using Cart.Application.Common;
using Cart.Application.Services;
using Cart.Domain.Entities;
using MediatR;

namespace Cart.Application.Queries.Me.GetMyCart;

public class GetMyCartQueryHandler(CartOperations cartOperations, ICurrentUserService currentUserService)
    : IRequestHandler<GetMyCartQuery, CartDto>
{
    public async Task<CartDto> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForUser(currentUserService.UserId!.Value);

        var cart = await cartOperations.GetCartAsync(owner, cancellationToken)
            ?? ShoppingCart.Create(currentUserService.UserId!.Value, null);

        return CartMapper.ToDto(cart);
    }
}
