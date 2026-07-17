using Review.Domain.Enums;
using Review.Domain.Exceptions;

namespace Review.Domain.Entities;

/// <summary>
/// A single user's rating/comment for a purchased item. Targets the exact (SellableItemId,
/// SellableItemType) pair the user was actually charged for — the same Product-or-Variant
/// distinction used by Cart/Order/Inventory/Pricing — rather than always rolling up to the parent
/// Product, so a review always corresponds 1:1 to something IOrderPurchaseVerifier can confirm was
/// bought. Per project-overview.md §4: purchasers only, published instantly, no moderation.
/// </summary>
public class ProductReview
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid SellableItemId { get; private set; }
    public ReviewItemType SellableItemType { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ProductReview()
    {
    }

    public static ProductReview Create(Guid userId, Guid sellableItemId, ReviewItemType sellableItemType, int rating, string comment)
    {
        ValidateRating(rating);

        return new ProductReview
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SellableItemId = sellableItemId,
            SellableItemType = sellableItemType,
            Rating = rating,
            Comment = comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(int rating, string comment)
    {
        ValidateRating(rating);

        Rating = rating;
        Comment = comment.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateRating(int rating)
    {
        if (rating is < 1 or > 5)
            throw new InvalidRatingException(rating);
    }
}
