using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.Anonymous.ClearAnonymousCart;

public class ClearAnonymousCartCommandHandler(CartOperations cartOperations)
    : IRequestHandler<ClearAnonymousCartCommand, Unit>
{
    public async Task<Unit> Handle(ClearAnonymousCartCommand request, CancellationToken cancellationToken)
    {
        var owner = CartOwnerKey.ForAnonymous(request.AnonymousId);

        await cartOperations.ClearAsync(owner, cancellationToken);

        return Unit.Value;
    }
}
