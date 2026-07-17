using BuildingBlocks.Application.Messaging;
using Review.Application.Common;
using Review.Domain.Enums;

namespace Review.Application.Queries.GetProductReviewsList;

public record GetProductReviewsListQuery(
    Guid SellableItemId,
    ReviewItemType SellableItemType,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResult<ReviewDto>>;
