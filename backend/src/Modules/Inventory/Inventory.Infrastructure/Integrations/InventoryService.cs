using Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;
using Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;
using Inventory.Application.Commands.Reservations.ReserveStock;
using Inventory.Application.Queries.Reservations.GetAvailableQuantity;
using Inventory.Contracts;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using MediatR;

namespace Inventory.Infrastructure.Integrations;

/// <summary>
/// Implementation of Inventory's outbound contract (<see cref="IInventoryService"/>).
/// Delegates to the same MediatR commands/queries used internally, instead of duplicating the
/// reservation business logic — Reserve/Confirm/Release require the same domain rules and
/// transaction handling regardless of caller.
/// </summary>
public class InventoryContractService(ISender sender) : IInventoryService
{
    public async Task<ReserveStockResult> ReserveStockAsync(
        Guid referenceId, IReadOnlyCollection<ReserveStockItemRequest> items, CancellationToken cancellationToken)
    {
        var lineItems = items
            .Select(i => new ReserveStockLineItem(i.SellableItemId, i.SellableItemType, i.Quantity))
            .ToList();

        try
        {
            await sender.Send(new ReserveStockCommand(referenceId, lineItems), cancellationToken);
            return new ReserveStockResult(true, referenceId, null);
        }
        catch (InsufficientStockException ex)
        {
            return new ReserveStockResult(false, referenceId, ex.Message);
        }
        catch (StockItemNotFoundException ex)
        {
            return new ReserveStockResult(false, referenceId, ex.Message);
        }
    }

    public Task ConfirmReservationsAsync(Guid referenceId, CancellationToken cancellationToken)
        => sender.Send(new ConfirmReservationsByReferenceCommand(referenceId), cancellationToken);

    public Task ReleaseReservationsAsync(Guid referenceId, CancellationToken cancellationToken)
        => sender.Send(new ReleaseReservationsByReferenceCommand(referenceId), cancellationToken);

    public Task<int> GetAvailableQuantityAsync(Guid sellableItemId, InventoryItemType sellableItemType, CancellationToken cancellationToken)
        => sender.Send(new GetAvailableQuantityQuery(sellableItemId, sellableItemType), cancellationToken);
}
