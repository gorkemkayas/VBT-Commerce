using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Common;
using Payment.Application.Queries.GetPaymentById;
using Payment.Application.Queries.GetPaymentsList;
using Payment.Domain.Enums;

namespace ECommerce.API.Controllers.Payments;

[ApiController]
[Route("api/admin/payments")]
public class AdminPaymentsController(ISender sender) : ControllerBase
{
    [HttpGet("{paymentId:guid}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid paymentId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(paymentId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PaymentDto>>> GetPaymentsList(
        [FromQuery] PaymentStatus? status, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetPaymentsListQuery(status, pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }
}
