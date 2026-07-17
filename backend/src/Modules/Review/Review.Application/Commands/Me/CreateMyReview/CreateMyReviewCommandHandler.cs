using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Application.Integrations;
using Review.Domain.Entities;
using Review.Domain.Exceptions;

namespace Review.Application.Commands.Me.CreateMyReview;

public class CreateMyReviewCommandHandler(
    IReviewDbContext dbContext,
    ICatalogIntegrationService catalogIntegrationService,
    IOrderIntegrationService orderIntegrationService,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateMyReviewCommand, Guid>
{
    public async Task<Guid> Handle(CreateMyReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var itemExists = await catalogIntegrationService.SellableItemExistsAsync(
            request.SellableItemId, request.SellableItemType, cancellationToken);
        if (!itemExists)
            throw new ReviewSellableItemNotFoundException(request.SellableItemId);

        var hasPurchased = await orderIntegrationService.HasPurchasedItemAsync(
            userId, request.SellableItemId, request.SellableItemType, cancellationToken);
        if (!hasPurchased)
            throw new ForbiddenException("Only customers who have purchased this item may review it.");

        var alreadyReviewed = await dbContext.Reviews.AnyAsync(
            r => r.UserId == userId && r.SellableItemId == request.SellableItemId && r.SellableItemType == request.SellableItemType,
            cancellationToken);
        if (alreadyReviewed)
            throw new DuplicateReviewException(userId, request.SellableItemId);

        var review = ProductReview.Create(userId, request.SellableItemId, request.SellableItemType, request.Rating, request.Comment);

        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}
