using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Application.Common;
using Review.Application.Integrations;

namespace Review.Application.Queries.GetProductReviewsList;

public class GetProductReviewsListQueryHandler(IReviewDbContext dbContext, IIdentityIntegrationService identityIntegrationService)
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

        var displayNames = new Dictionary<Guid, string?>();
        foreach (var userId in items.Select(r => r.UserId).Distinct())
            displayNames[userId] = await identityIntegrationService.GetMaskedDisplayNameAsync(userId, cancellationToken);

        var dtos = items.Select(r => ReviewMapper.ToDto(r, displayNames[r.UserId])).ToList();

        return new PagedResult<ReviewDto>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
