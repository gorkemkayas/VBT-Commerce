using Cart.Application.Commands.Anonymous.AddItemToAnonymousCart;
using Cart.Application.Commands.Anonymous.ClearAnonymousCart;
using Cart.Application.Commands.Anonymous.RemoveAnonymousCartItem;
using Cart.Application.Commands.Anonymous.UpdateAnonymousCartItemQuantity;
using Cart.Application.Common;
using Cart.Application.Queries.Anonymous.GetAnonymousCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Carts;

// Public — no authentication. AnonymousId is a client-generated correlation key (see architecture
// decision in Cart.Application.Commands.Anonymous), not a security boundary.
[ApiController]
[Route("api/carts/anonymous/{anonymousId:guid}")]
public class AnonymousCartController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(Guid anonymousId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAnonymousCartQuery(anonymousId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<ActionResult<Guid>> AddItem(Guid anonymousId, AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var itemId = await sender.Send(
            new AddItemToAnonymousCartCommand(anonymousId, request.SellableItemId, request.SellableItemType, request.Quantity),
            cancellationToken);

        return CreatedAtAction(nameof(GetCart), new { anonymousId }, itemId);
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItemQuantity(
        Guid anonymousId, Guid itemId, UpdateCartItemQuantityRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateAnonymousCartItemQuantityCommand(anonymousId, itemId, request.Quantity), cancellationToken);
        return NoContent();
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid anonymousId, Guid itemId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveAnonymousCartItemCommand(anonymousId, itemId), cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(Guid anonymousId, CancellationToken cancellationToken)
    {
        await sender.Send(new ClearAnonymousCartCommand(anonymousId), cancellationToken);
        return NoContent();
    }
}
