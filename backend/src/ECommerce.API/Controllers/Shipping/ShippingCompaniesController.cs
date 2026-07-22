using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;
using Shipping.Application.Commands.ShippingCompanies.DeactivateShippingCompany;
using Shipping.Application.Commands.ShippingCompanies.UpdateShippingCompany;
using Shipping.Application.Common;
using Shipping.Application.Queries.ShippingCompanies.GetActiveShippingCompanies;
using Shipping.Application.Queries.ShippingCompanies.GetShippingCompaniesList;
using Shipping.Application.Queries.ShippingCompanies.GetShippingCompanyById;

using ECommerce.API.Controllers.Shipping.Requests;

namespace ECommerce.API.Controllers.Shipping;

[ApiController]
public class ShippingCompaniesController(ISender sender) : ControllerBase
{
    [HttpPost("api/admin/shipping-companies")]
    public async Task<ActionResult<Guid>> CreateShippingCompany(CreateShippingCompanyRequest request, CancellationToken cancellationToken)
    {
        var shippingCompanyId = await sender.Send(
            new CreateShippingCompanyCommand(request.Name, request.Fee), cancellationToken);

        return CreatedAtAction(nameof(GetShippingCompanyById), new { shippingCompanyId }, shippingCompanyId);
    }

    [HttpPut("api/admin/shipping-companies/{shippingCompanyId:guid}")]
    public async Task<IActionResult> UpdateShippingCompany(Guid shippingCompanyId, UpdateShippingCompanyRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateShippingCompanyCommand(shippingCompanyId, request.Name, request.Fee), cancellationToken);
        return NoContent();
    }

    [HttpPost("api/admin/shipping-companies/{shippingCompanyId:guid}/deactivate")]
    public async Task<IActionResult> DeactivateShippingCompany(Guid shippingCompanyId, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateShippingCompanyCommand(shippingCompanyId), cancellationToken);
        return NoContent();
    }

    [HttpGet("api/admin/shipping-companies/{shippingCompanyId:guid}")]
    public async Task<ActionResult<ShippingCompanyDto>> GetShippingCompanyById(Guid shippingCompanyId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetShippingCompanyByIdQuery(shippingCompanyId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/admin/shipping-companies")]
    public async Task<ActionResult<PagedResult<ShippingCompanyDto>>> GetShippingCompaniesList(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetShippingCompaniesListQuery(
            pageNumber == 0 ? 1 : pageNumber,
            pageSize == 0 ? 20 : pageSize);

        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/shipping-companies")]
    public async Task<ActionResult<IReadOnlyList<ShippingCompanyDto>>> GetActiveShippingCompanies(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetActiveShippingCompaniesQuery(), cancellationToken);
        return Ok(result);
    }
}
