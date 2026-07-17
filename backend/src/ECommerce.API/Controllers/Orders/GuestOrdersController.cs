using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands.Checkout.PlaceGuestOrder;
using Order.Application.Common;
using Order.Application.Queries.Guest.GetGuestOrderById;

namespace ECommerce.API.Controllers.Orders;

[ApiController]
[Route("api/orders/guest")]
public class GuestOrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> PlaceOrder(PlaceGuestOrderRequest request, CancellationToken cancellationToken)
    {
        var orderId = await sender.Send(
            new PlaceGuestOrderCommand(
                request.GuestCustomerId,
                request.AnonymousId,
                request.ShippingCompanyId,
                request.CouponCodes,
                request.RecipientName,
                request.PhoneNumber,
                request.Country,
                request.City,
                request.District,
                request.PostalCode,
                request.AddressLine1,
                request.AddressLine2),
            cancellationToken);

        return CreatedAtAction(nameof(GetOrderById), new { guestCustomerId = request.GuestCustomerId, orderId }, orderId);
    }

    [HttpGet("{guestCustomerId:guid}/{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid guestCustomerId, Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGuestOrderByIdQuery(guestCustomerId, orderId), cancellationToken);
        return Ok(result);
    }
}
