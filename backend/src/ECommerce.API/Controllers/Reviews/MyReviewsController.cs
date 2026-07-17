using MediatR;
using Microsoft.AspNetCore.Mvc;
using Review.Application.Commands.Me.CreateMyReview;
using Review.Application.Commands.Me.DeleteMyReview;
using Review.Application.Commands.Me.UpdateMyReview;
using Review.Application.Common;
using Review.Application.Queries.Me.GetMyReviewsList;

namespace ECommerce.API.Controllers.Reviews;

[ApiController]
[Route("api/reviews/me")]
public class MyReviewsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetMyReviews(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyReviewsListQuery(pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var reviewId = await sender.Send(
            new CreateMyReviewCommand(request.SellableItemId, request.SellableItemType, request.Rating, request.Comment),
            cancellationToken);

        return CreatedAtAction(nameof(GetMyReviews), null, reviewId);
    }

    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> Update(Guid reviewId, UpdateReviewRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateMyReviewCommand(reviewId, request.Rating, request.Comment), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> Delete(Guid reviewId, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteMyReviewCommand(reviewId), cancellationToken);
        return NoContent();
    }
}
