using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pricing.Application.Commands.TaxRate.UpdateTaxRate;
using Pricing.Application.Queries.TaxRate.GetTaxRate;

namespace ECommerce.API.Controllers.Pricing;

[ApiController]
public class TaxRateController(ISender sender) : ControllerBase
{
    [HttpGet("api/tax-rate")]
    public async Task<ActionResult<decimal>> GetTaxRate(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTaxRateQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("api/admin/tax-rate")]
    public async Task<IActionResult> UpdateTaxRate(UpdateTaxRateRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateTaxRateCommand(request.Rate), cancellationToken);
        return NoContent();
    }
}
