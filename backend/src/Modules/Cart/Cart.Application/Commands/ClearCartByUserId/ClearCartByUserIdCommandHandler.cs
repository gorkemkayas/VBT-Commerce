using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.ClearCartByUserId;

public class ClearCartByUserIdCommandHandler(CartOperations cartOperations)
    : IRequestHandler<ClearCartByUserIdCommand, Unit>
{
    public async Task<Unit> Handle(ClearCartByUserIdCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForUser(request.UserId);

        await cartOperations.ClearAsync(owner, cancellationToken);

        return Unit.Value;
    }
}
