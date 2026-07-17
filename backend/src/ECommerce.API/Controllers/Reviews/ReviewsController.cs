using MediatR;
using Microsoft.AspNetCore.Mvc;
using Review.Application.Common;
using Review.Application.Queries.GetProductReviewSummary;
using Review.Application.Queries.GetProductReviewsList;
using Review.Domain.Enums;

namespace ECommerce.API.Controllers.Reviews;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetList(
        [FromQuery] Guid sellableItemId, [FromQuery] ReviewItemType sellableItemType,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProductReviewsListQuery(sellableItemId, sellableItemType, pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ReviewSummaryDto>> GetSummary(
        [FromQuery] Guid sellableItemId, [FromQuery] ReviewItemType sellableItemType, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductReviewSummaryQuery(sellableItemId, sellableItemType), cancellationToken);
        return Ok(result);
    }
}
