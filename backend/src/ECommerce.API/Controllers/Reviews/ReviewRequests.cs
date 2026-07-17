using Review.Domain.Enums;

namespace ECommerce.API.Controllers.Reviews;

public record CreateReviewRequest(Guid SellableItemId, ReviewItemType SellableItemType, int Rating, string Comment);

public record UpdateReviewRequest(int Rating, string Comment);
