using Cart.Contracts;

namespace Order.Application.Integrations;

public interface ICartIntegrationService
{
    Task<CartSummaryDto?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<CartSummaryDto?> GetCartByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken);

    Task ClearByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task ClearByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken);
}
