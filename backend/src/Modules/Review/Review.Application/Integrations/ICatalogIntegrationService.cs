using Review.Domain.Enums;

namespace Review.Application.Integrations;

/// <summary>
/// Review's own view of what it needs from the Catalog module (architecture.md §3 — integrations/
/// consume another module's contract). Implemented in Review.Infrastructure against Catalog's
/// IProductCatalogService, so Review.Application never depends on Catalog directly.
/// </summary>
public interface ICatalogIntegrationService
{
    Task<bool> SellableItemExistsAsync(Guid sellableItemId, ReviewItemType sellableItemType, CancellationToken cancellationToken);
}
