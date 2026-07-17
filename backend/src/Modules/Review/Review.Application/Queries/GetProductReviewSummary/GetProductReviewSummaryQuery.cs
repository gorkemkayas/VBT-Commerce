using BuildingBlocks.Application.Messaging;
using Review.Application.Common;
using Review.Domain.Enums;

namespace Review.Application.Queries.GetProductReviewSummary;

public record GetProductReviewSummaryQuery(Guid SellableItemId, ReviewItemType SellableItemType) : IQuery<ReviewSummaryDto>;
