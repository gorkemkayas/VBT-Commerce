using Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;
using Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;
using Inventory.Application.Commands.Reservations.ReserveStock;
using Inventory.Application.Queries.Reservations.GetAvailableQuantity;
using Inventory.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Inventory;

/// <summary>
/// Reserve/Confirm/Release wrap MediatR commands that intentionally carry no <c>IRequireRole</c> —
/// they are meant to be invoked in-process by other modules (Order) on behalf of a customer's own
/// checkout request. Direct HTTP access to them here is an administrative/support affordance only,
/// gated with the native ASP.NET Core [Authorize] attribute instead of the MediatR pipeline, so it
/// never blocks Order's future in-process calls.
/// </summary>
[ApiController]
[Route("api/stock-reservations")]
public class ReservationsController(ISender sender) : ControllerBase
{
    [HttpGet("available-quantity/{sellableItemId:guid}")]
    public async Task<ActionResult<int>> GetAvailableQuantity(
        Guid sellableItemId, [FromQuery] InventoryItemType sellableItemType, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAvailableQuantityQuery(sellableItemId, sellableItemType), cancellationToken);
        return Ok(result);
    }

    [HttpPost("reserve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reserve(ReserveStockRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items
            .Select(i => new ReserveStockLineItem(i.SellableItemId, i.SellableItemType, i.Quantity))
            .ToList();

        await sender.Send(new ReserveStockCommand(request.ReferenceId, items), cancellationToken);
        return NoContent();
    }

    [HttpPost("{referenceId:guid}/confirm")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Confirm(Guid referenceId, CancellationToken cancellationToken)
    {
        await sender.Send(new ConfirmReservationsByReferenceCommand(referenceId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{referenceId:guid}/release")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Release(Guid referenceId, CancellationToken cancellationToken)
    {
        await sender.Send(new ReleaseReservationsByReferenceCommand(referenceId), cancellationToken);
        return NoContent();
    }
}
