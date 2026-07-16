using Cart.Application.Services;
using MediatR;

namespace Cart.Application.Commands.MergeAnonymousCart;

public class MergeAnonymousCartCommandHandler(CartOperations cartOperations) : IRequestHandler<MergeAnonymousCartCommand, Unit>
{
    public async Task<Unit> Handle(MergeAnonymousCartCommand request, CancellationToken cancellationToken)
    {
        await cartOperations.MergeAnonymousCartIntoUserCartAsync(request.UserId, request.AnonymousId, cancellationToken);

        return Unit.Value;
    }
}
