namespace Cart.Contracts;

public interface ICartReader
{
    Task<CartSummaryDto?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<CartSummaryDto?> GetCartByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken);
}
