using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Application.Common;

namespace Review.Application.Queries.GetProductReviewsList;

public class GetProductReviewsListQueryHandler(IReviewDbContext dbContext)
    : IRequestHandler<GetProductReviewsListQuery, PagedResult<ReviewDto>>
{
    public async Task<PagedResult<ReviewDto>> Handle(GetProductReviewsListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Reviews.AsNoTracking()
            .Where(r => r.SellableItemId == request.SellableItemId && r.SellableItemType == request.SellableItemType);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ReviewDto>(items.Select(ReviewMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
