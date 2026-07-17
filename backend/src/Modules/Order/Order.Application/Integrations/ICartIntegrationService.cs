using Cart.Contracts;

namespace Order.Application.Integrations;

public interface ICartIntegrationService
{
    Task<CartSummaryDto?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<CartSummaryDto?> GetCartByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken);
}
