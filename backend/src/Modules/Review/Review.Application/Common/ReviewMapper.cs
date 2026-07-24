using Review.Domain.Entities;

namespace Review.Application.Common;

public static class ReviewMapper
{
    public static ReviewDto ToDto(ProductReview review, string? reviewerDisplayName = null) => new(
        review.Id,
        review.UserId,
        review.SellableItemId,
        review.SellableItemType,
        review.Rating,
        review.Comment,
        review.CreatedAt,
        review.UpdatedAt,
        reviewerDisplayName);
}
