using Review.Domain.Enums;

namespace Review.Application.Integrations;

/// <summary>
/// Review's own view of what it needs from the Order module. Implemented in Review.Infrastructure
/// against Order's IOrderPurchaseVerifier — per project-overview.md §4, only a customer who
/// purchased (and had a Confirmed order for) an item may review it.
/// </summary>
public interface IOrderIntegrationService
{
    Task<bool> HasPurchasedItemAsync(Guid userId, Guid sellableItemId, ReviewItemType sellableItemType, CancellationToken cancellationToken);
}
