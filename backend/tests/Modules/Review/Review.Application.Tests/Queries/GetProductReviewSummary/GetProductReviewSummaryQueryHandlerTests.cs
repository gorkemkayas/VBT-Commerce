using FluentAssertions;
using Review.Application.Queries.GetProductReviewSummary;
using Review.Domain.Entities;
using Review.Domain.Enums;
using Xunit;

namespace Review.Application.Tests.Queries.GetProductReviewSummary;

public class GetProductReviewSummaryQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoReviews_ReturnsZeroAverageAndCount()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        var handler = new GetProductReviewSummaryQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductReviewSummaryQuery(itemId, ReviewItemType.Product), CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.AverageRating.Should().Be(0);
        result.SellableItemId.Should().Be(itemId);
        result.SellableItemType.Should().Be(ReviewItemType.Product);
    }

    [Fact]
    public async Task Handle_WithMultipleReviews_ReturnsRoundedAverageAndCount()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        dbContext.Reviews.AddRange(
            ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 5, "A"),
            ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 4, "B"),
            ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 4, "C"));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductReviewSummaryQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductReviewSummaryQuery(itemId, ReviewItemType.Product), CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.AverageRating.Should().Be(4.33);
    }

    [Fact]
    public async Task Handle_WithReviewsForDifferentSellableItemType_ExcludesThemFromSummary()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        dbContext.Reviews.AddRange(
            ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 5, "Product review"),
            ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Variant, 1, "Variant review"));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductReviewSummaryQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductReviewSummaryQuery(itemId, ReviewItemType.Product), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.AverageRating.Should().Be(5);
    }
}
