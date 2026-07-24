using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Application.Integrations;
using Review.Domain.Entities;
using Review.Domain.Enums;
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

        // A product can also be reviewed by someone who bought one of its variants rather than the
        // bare product itself — Order only knows about variant purchases, not their parent product,
        // so resolve that link here via Catalog.
        if (!hasPurchased && request.SellableItemType == ReviewItemType.Product)
        {
            var purchasedVariantIds = await orderIntegrationService.GetPurchasedVariantIdsAsync(userId, cancellationToken);
            foreach (var variantId in purchasedVariantIds)
            {
                var parentProductId = await catalogIntegrationService.GetVariantProductIdAsync(variantId, cancellationToken);
                if (parentProductId != request.SellableItemId)
                    continue;

                hasPurchased = true;
                break;
            }
        }

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
