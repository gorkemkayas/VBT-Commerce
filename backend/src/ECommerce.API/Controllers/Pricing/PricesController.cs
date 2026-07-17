using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pricing.Application.Commands.Prices.CreatePrice;
using Pricing.Application.Commands.Prices.UpdatePrice;
using Pricing.Application.Common;
using Pricing.Application.Queries.Prices.GetPrice;
using Pricing.Domain.Enums;

namespace ECommerce.API.Controllers.Pricing;

[ApiController]
public class PricesController(ISender sender) : ControllerBase
{
    [HttpPost("api/admin/prices")]
    public async Task<ActionResult<Guid>> CreatePrice(CreatePriceRequest request, CancellationToken cancellationToken)
    {
        var priceId = await sender.Send(
            new CreatePriceCommand(request.SellableItemId, request.SellableItemType, request.Amount), cancellationToken);

        return CreatedAtAction(
            nameof(GetPrice), new { sellableItemType = request.SellableItemType, sellableItemId = request.SellableItemId }, priceId);
    }

    [HttpPut("api/admin/prices/{priceId:guid}")]
    public async Task<IActionResult> UpdatePrice(Guid priceId, UpdatePriceRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdatePriceCommand(priceId, request.Amount), cancellationToken);
        return NoContent();
    }

    [HttpGet("api/prices/{sellableItemType}/{sellableItemId:guid}")]
    public async Task<ActionResult<PriceDto>> GetPrice(PriceItemType sellableItemType, Guid sellableItemId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPriceQuery(sellableItemId, sellableItemType), cancellationToken);
        return Ok(result);
    }
}
