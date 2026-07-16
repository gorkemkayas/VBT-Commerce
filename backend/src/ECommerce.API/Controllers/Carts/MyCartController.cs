using Cart.Application.Commands.Me.AddItemToMyCart;
using Cart.Application.Commands.Me.ClearMyCart;
using Cart.Application.Commands.Me.RemoveMyCartItem;
using Cart.Application.Commands.Me.UpdateMyCartItemQuantity;
using Cart.Application.Common;
using Cart.Application.Queries.Me.GetMyCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Carts;

[ApiController]
[Route("api/carts/me")]
public class MyCartController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetMyCart(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyCartQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<ActionResult<Guid>> AddItem(AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var itemId = await sender.Send(
            new AddItemToMyCartCommand(request.SellableItemId, request.SellableItemType, request.Quantity), cancellationToken);

        return CreatedAtAction(nameof(GetMyCart), null, itemId);
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItemQuantity(Guid itemId, UpdateCartItemQuantityRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateMyCartItemQuantityCommand(itemId, request.Quantity), cancellationToken);
        return NoContent();
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveMyCartItemCommand(itemId), cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        await sender.Send(new ClearMyCartCommand(), cancellationToken);
        return NoContent();
    }
}
