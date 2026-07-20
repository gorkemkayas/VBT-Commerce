using Inventory.Contracts;
using Order.Application.Integrations;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using InventoryItemType = Inventory.Domain.Enums.InventoryItemType;

namespace Order.Infrastructure.Integrations;

/// <summary>
/// Adapts Order's own <see cref="IInventoryIntegrationService"/> to Inventory's actual contract
/// (<see cref="IInventoryService"/>), so Order.Application never references Inventory.Application directly.
/// </summary>
public class InventoryIntegrationService(IInventoryService inventoryService) : IInventoryIntegrationService
{
    public async Task ReserveStockAsync(
        Guid referenceId, IReadOnlyCollection<ReserveStockLineItem> items, CancellationToken cancellationToken)
    {
        var requestItems = items
            .Select(i => new ReserveStockItemRequest(i.SellableItemId, MapToInventoryItemType(i.SellableItemType), i.Quantity))
            .ToList();

        var result = await inventoryService.ReserveStockAsync(referenceId, requestItems, cancellationToken);

        if (!result.IsSuccess)
            throw new OrderInsufficientStockException(result.ErrorMessage ?? "Unknown reason.");
    }

    public Task ConfirmReservationsAsync(Guid referenceId, CancellationToken cancellationToken)
        => inventoryService.ConfirmReservationsAsync(referenceId, cancellationToken);

    public Task ReleaseReservationsAsync(Guid referenceId, CancellationToken cancellationToken)
        => inventoryService.ReleaseReservationsAsync(referenceId, cancellationToken);

    private static InventoryItemType MapToInventoryItemType(OrderItemType type) => type switch
    {
        OrderItemType.Product => InventoryItemType.Product,
        OrderItemType.Variant => InventoryItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
