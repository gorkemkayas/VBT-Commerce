using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Review.Application.Common;

namespace Review.Application.Queries.Me.GetMyReviewsList;

public record GetMyReviewsListQuery(int PageNumber = 1, int PageSize = 20) : IQuery<PagedResult<ReviewDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
