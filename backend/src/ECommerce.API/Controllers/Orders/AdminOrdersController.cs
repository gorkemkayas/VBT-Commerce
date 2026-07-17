using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands.Admin.CancelOrder;
using Order.Application.Commands.Admin.ConfirmOrder;
using Order.Application.Common;
using Order.Application.Queries.Admin.GetOrderById;
using Order.Application.Queries.Admin.GetOrdersList;
using Order.Domain.Enums;

namespace ECommerce.API.Controllers.Orders;

[ApiController]
[Route("api/admin/orders")]
public class AdminOrdersController(ISender sender) : ControllerBase
{
    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(orderId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrdersList(
        [FromQuery] OrderStatus? status, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetOrdersListQuery(status, pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:guid}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId, CancellationToken cancellationToken)
    {
        await sender.Send(new ConfirmOrderCommand(orderId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancelOrderRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelOrderCommand(orderId, request.Reason), cancellationToken);
        return NoContent();
    }
}
