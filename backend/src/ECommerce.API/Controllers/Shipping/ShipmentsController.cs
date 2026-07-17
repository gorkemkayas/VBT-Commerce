using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipping.Application.Commands.Shipments.UpdateShipmentStatus;
using Shipping.Application.Common;
using Shipping.Application.Queries.Shipments.GetShipmentById;
using Shipping.Application.Queries.Shipments.GetShipmentsList;
using Shipping.Domain.Enums;

namespace ECommerce.API.Controllers.Shipping;

[ApiController]
[Route("api/admin/shipments")]
public class ShipmentsController(ISender sender) : ControllerBase
{
    [HttpGet("{shipmentId:guid}")]
    public async Task<ActionResult<ShipmentDto>> GetShipmentById(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetShipmentByIdQuery(shipmentId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ShipmentDto>>> GetShipmentsList(
        [FromQuery] ShipmentStatus? status, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetShipmentsListQuery(
            status,
            pageNumber == 0 ? 1 : pageNumber,
            pageSize == 0 ? 20 : pageSize);

        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{shipmentId:guid}/status")]
    public async Task<IActionResult> UpdateShipmentStatus(Guid shipmentId, UpdateShipmentStatusRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateShipmentStatusCommand(shipmentId, request.Status, request.TrackingNumber), cancellationToken);
        return NoContent();
    }
}
