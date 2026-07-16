using Cart.Application.Common;
using Cart.Application.Services;
using Cart.Domain.Entities;
using MediatR;

namespace Cart.Application.Queries.Anonymous.GetAnonymousCart;

public class GetAnonymousCartQueryHandler(CartOperations cartOperations) : IRequestHandler<GetAnonymousCartQuery, CartDto>
{
    public async Task<CartDto> Handle(GetAnonymousCartQuery request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForAnonymous(request.AnonymousId);

        var cart = await cartOperations.GetCartAsync(owner, cancellationToken)
            ?? ShoppingCart.Create(null, request.AnonymousId);

        return CartMapper.ToDto(cart);
    }
}
