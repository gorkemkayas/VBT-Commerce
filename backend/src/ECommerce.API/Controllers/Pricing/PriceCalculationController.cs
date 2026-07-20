using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;
using Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;
using Pricing.Contracts;

namespace ECommerce.API.Controllers.Pricing;

[ApiController]
[Route("api/pricing/calculate")]
public class PriceCalculationController(ISender sender) : ControllerBase
{
    [HttpPost("me")]
    public async Task<ActionResult<PriceCalculationResultDto>> CalculateMyOrderPrice(
        CalculateMyOrderPriceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CalculateMyOrderPriceQuery(request.Items, request.CouponCodes), cancellationToken);
        return Ok(result);
    }

    [HttpPost("guest")]
    public async Task<ActionResult<PriceCalculationResultDto>> CalculateGuestOrderPrice(
        CalculateGuestOrderPriceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CalculateGuestOrderPriceQuery(request.GuestCustomerId, request.Items, request.CouponCodes), cancellationToken);

        return Ok(result);
    }
}
