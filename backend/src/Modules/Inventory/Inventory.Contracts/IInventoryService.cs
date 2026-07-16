using Inventory.Domain.Enums;

namespace Inventory.Contracts;

/// <summary>
/// The Inventory module's outbound contract (architecture.md §3). Other modules — chiefly Order,
/// during the synchronous stock check/reservation step of checkout — depend on this instead of
/// Inventory's internal persistence. See architecture.md §6.1 for the intended call sequence:
/// ReserveStockAsync before the Order is created, ConfirmReservationsAsync once payment succeeds,
/// ReleaseReservationsAsync as the compensating action if order creation fails afterwards.
/// </summary>
public interface IInventoryService
{
    Task<ReserveStockResult> ReserveStockAsync(Guid referenceId, IReadOnlyCollection<ReserveStockItemRequest> items, CancellationToken cancellationToken);

    Task ConfirmReservationsAsync(Guid referenceId, CancellationToken cancellationToken);

    Task ReleaseReservationsAsync(Guid referenceId, CancellationToken cancellationToken);

    Task<int> GetAvailableQuantityAsync(Guid sellableItemId, InventoryItemType sellableItemType, CancellationToken cancellationToken);
}
