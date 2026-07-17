using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands.Checkout.PlaceMyOrder;
using Order.Application.Commands.Me.CancelMyOrder;
using Order.Application.Common;
using Order.Application.Queries.Me.GetMyOrderById;
using Order.Application.Queries.Me.GetMyOrdersList;

namespace ECommerce.API.Controllers.Orders;

[ApiController]
[Route("api/orders/me")]
public class MyOrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> PlaceOrder(PlaceMyOrderRequest request, CancellationToken cancellationToken)
    {
        var orderId = await sender.Send(
            new PlaceMyOrderCommand(
                request.AddressId,
                request.ShippingCompanyId,
                request.CouponCodes,
                request.CardHolderName,
                request.CardNumber,
                request.CardExpireMonth,
                request.CardExpireYear,
                request.CardCvc,
                request.BuyerIdentityNumber),
            cancellationToken);

        return CreatedAtAction(nameof(GetOrderById), new { orderId }, orderId);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyOrderByIdQuery(orderId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrdersList(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetMyOrdersListQuery(pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelMyOrderCommand(orderId), cancellationToken);
        return NoContent();
    }
}
