namespace Order.Application.Integrations;

/// <summary>
/// Order's own view of what it needs from the Inventory module during checkout (architecture.md
/// §6.1 — reserve before the Order is created, confirm once payment succeeds, release as the
/// compensating action). Implemented in Order.Infrastructure against Inventory.Contracts.IInventoryService,
/// so Order.Application never references Inventory.Application directly.
/// </summary>
public interface IInventoryIntegrationService
{
    Task ReserveStockAsync(Guid referenceId, IReadOnlyCollection<ReserveStockLineItem> items, CancellationToken cancellationToken);

    Task ConfirmReservationsAsync(Guid referenceId, CancellationToken cancellationToken);

    Task ReleaseReservationsAsync(Guid referenceId, CancellationToken cancellationToken);
}
