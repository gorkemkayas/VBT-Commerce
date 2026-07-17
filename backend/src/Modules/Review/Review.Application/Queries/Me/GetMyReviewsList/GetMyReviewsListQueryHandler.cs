using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Application.Common;

namespace Review.Application.Queries.Me.GetMyReviewsList;

public class GetMyReviewsListQueryHandler(IReviewDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<GetMyReviewsListQuery, PagedResult<ReviewDto>>
{
    public async Task<PagedResult<ReviewDto>> Handle(GetMyReviewsListQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var query = dbContext.Reviews.AsNoTracking().Where(r => r.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ReviewDto>(items.Select(ReviewMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
