using Cart.Contracts;
using Order.Application.Integrations;

namespace Order.Infrastructure.Integrations;

public class CartIntegrationService(ICartReader cartReader, ICartMutator cartMutator) : ICartIntegrationService
{
    public Task<CartSummaryDto?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => cartReader.GetCartByUserIdAsync(userId, cancellationToken);

    public Task<CartSummaryDto?> GetCartByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken)
        => cartReader.GetCartByAnonymousIdAsync(anonymousId, cancellationToken);

    public Task ClearByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => cartMutator.ClearByUserIdAsync(userId, cancellationToken);

    public Task ClearByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken)
        => cartMutator.ClearByAnonymousIdAsync(anonymousId, cancellationToken);
}
