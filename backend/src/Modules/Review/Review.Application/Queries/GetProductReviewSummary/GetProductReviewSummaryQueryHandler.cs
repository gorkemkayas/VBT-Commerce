using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Application.Common;

namespace Review.Application.Queries.GetProductReviewSummary;

public class GetProductReviewSummaryQueryHandler(IReviewDbContext dbContext)
    : IRequestHandler<GetProductReviewSummaryQuery, ReviewSummaryDto>
{
    public async Task<ReviewSummaryDto> Handle(GetProductReviewSummaryQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Reviews.AsNoTracking()
            .Where(r => r.SellableItemId == request.SellableItemId && r.SellableItemType == request.SellableItemType);

        var totalCount = await query.CountAsync(cancellationToken);
        var averageRating = totalCount == 0 ? 0 : await query.AverageAsync(r => r.Rating, cancellationToken);

        return new ReviewSummaryDto(request.SellableItemId, request.SellableItemType, Math.Round(averageRating, 2), totalCount);
    }
}
