using BuildingBlocks.Application.Security;
using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Me.ClearMyCart;

public class ClearMyCartCommandHandler(CartOperations cartOperations, ICurrentUserService currentUserService)
    : IRequestHandler<ClearMyCartCommand, Unit>
{
    public async Task<Unit> Handle(ClearMyCartCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForUser(currentUserService.UserId!.Value);

        await cartOperations.ClearAsync(owner, cancellationToken);

        return Unit.Value;
    }
}
