using Inventory.Application.Commands.StockItems.CreateStockItem;
using Inventory.Application.Commands.StockItems.DecreaseStock;
using Inventory.Application.Commands.StockItems.IncreaseStock;
using Inventory.Application.Common;
using Inventory.Application.Queries.StockItems.GetStockItemById;
using Inventory.Application.Queries.StockItems.GetStockItemBySellableItem;
using Inventory.Application.Queries.StockItems.GetStockItemsList;
using Inventory.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using ECommerce.API.Controllers.Inventory.Requests;

namespace ECommerce.API.Controllers.Inventory;

[ApiController]
[Route("api/stock-items")]
public class StockItemsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<StockItemDto>>> GetList(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] InventoryItemType? sellableItemType,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetStockItemsListQuery(pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize, sellableItemType),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{stockItemId:guid}")]
    public async Task<ActionResult<StockItemDto>> GetById(Guid stockItemId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetStockItemByIdQuery(stockItemId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-sellable-item/{sellableItemId:guid}")]
    public async Task<ActionResult<StockItemDto>> GetBySellableItem(
        Guid sellableItemId, [FromQuery] InventoryItemType sellableItemType, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetStockItemBySellableItemQuery(sellableItemId, sellableItemType), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateStockItemRequest request, CancellationToken cancellationToken)
    {
        var stockItemId = await sender.Send(
            new CreateStockItemCommand(request.SellableItemId, request.SellableItemType, request.InitialQuantity),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { stockItemId }, stockItemId);
    }

    [HttpPost("{stockItemId:guid}/increase")]
    public async Task<IActionResult> IncreaseStock(Guid stockItemId, AdjustStockQuantityRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new IncreaseStockCommand(stockItemId, request.Quantity), cancellationToken);
        return NoContent();
    }

    [HttpPost("{stockItemId:guid}/decrease")]
    public async Task<IActionResult> DecreaseStock(Guid stockItemId, AdjustStockQuantityRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new DecreaseStockCommand(stockItemId, request.Quantity), cancellationToken);
        return NoContent();
    }
}
