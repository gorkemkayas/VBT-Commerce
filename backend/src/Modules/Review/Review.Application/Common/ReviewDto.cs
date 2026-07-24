using Review.Domain.Enums;

namespace Review.Application.Common;

public record ReviewDto(
    Guid Id,
    Guid UserId,
    Guid SellableItemId,
    ReviewItemType SellableItemType,
    int Rating,
    string Comment,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? ReviewerDisplayName = null);

public record ReviewSummaryDto(Guid SellableItemId, ReviewItemType SellableItemType, double AverageRating, int TotalCount);
