using FluentAssertions;
using Moq;
using Review.Application.Integrations;
using Review.Application.Queries.GetProductReviewsList;
using Review.Domain.Entities;
using Review.Domain.Enums;
using Xunit;

namespace Review.Application.Tests.Queries.GetProductReviewsList;

public class GetProductReviewsListQueryHandlerTests
{
    private readonly Mock<IIdentityIntegrationService> _identityIntegrationService = new();

    public GetProductReviewsListQueryHandlerTests()
    {
        _identityIntegrationService
            .Setup(x => x.GetMaskedDisplayNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("J*** D***");
    }

    [Fact]
    public async Task Handle_WithReviewsForItem_ReturnsPagedResultOrderedByNewestFirstWithMaskedNames()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        var older = ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 3, "Older review");
        await Task.Delay(10);
        var newer = ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 5, "Newer review");
        dbContext.Reviews.AddRange(older, newer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductReviewsListQueryHandler(dbContext, _identityIntegrationService.Object);

        var result = await handler.Handle(
            new GetProductReviewsListQuery(itemId, ReviewItemType.Product, 1, 20), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Id.Should().Be(newer.Id);
        result.Items.Last().Id.Should().Be(older.Id);
        result.Items.Should().OnlyContain(r => r.ReviewerDisplayName == "J*** D***");
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsRequestedPageOnly()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        for (var i = 0; i < 5; i++)
        {
            dbContext.Reviews.Add(ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 3, $"Review {i}"));
            await Task.Delay(5);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductReviewsListQueryHandler(dbContext, _identityIntegrationService.Object);

        var result = await handler.Handle(
            new GetProductReviewsListQuery(itemId, ReviewItemType.Product, 2, 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithReviewsForDifferentItem_ExcludesThemFromResult()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();
        var otherItemId = Guid.NewGuid();

        dbContext.Reviews.Add(ProductReview.Create(Guid.NewGuid(), itemId, ReviewItemType.Product, 3, "Mine"));
        dbContext.Reviews.Add(ProductReview.Create(Guid.NewGuid(), otherItemId, ReviewItemType.Product, 3, "Not mine"));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductReviewsListQueryHandler(dbContext, _identityIntegrationService.Object);

        var result = await handler.Handle(
            new GetProductReviewsListQuery(itemId, ReviewItemType.Product, 1, 20), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().SellableItemId.Should().Be(itemId);
    }
}
