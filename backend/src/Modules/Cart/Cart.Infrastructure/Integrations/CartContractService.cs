using System.Linq.Expressions;
using Cart.Contracts;
using Cart.Domain.Entities;
using Cart.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Integrations;

/// <summary>
/// Implementation of the Cart module's outbound contract (see architecture.md §3 —
/// contracts/integrations). Read-only: other modules consume this instead of touching
/// <see cref="CartDbContext"/> directly.
/// </summary>
public class CartContractService(CartDbContext dbContext) : ICartReader
{
    public Task<CartSummaryDto?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => GetCartAsync(c => c.UserId == userId, cancellationToken);

    public Task<CartSummaryDto?> GetCartByAnonymousIdAsync(Guid anonymousId, CancellationToken cancellationToken)
        => GetCartAsync(c => c.AnonymousId == anonymousId, cancellationToken);

    private async Task<CartSummaryDto?> GetCartAsync(
        Expression<Func<ShoppingCart, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.Carts
            .AsNoTracking()
            .Where(predicate)
            .Select(c => new CartSummaryDto(
                c.Id,
                c.Items.Select(i => new CartItemSummaryDto(i.SellableItemId, i.SellableItemType, i.Quantity)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
