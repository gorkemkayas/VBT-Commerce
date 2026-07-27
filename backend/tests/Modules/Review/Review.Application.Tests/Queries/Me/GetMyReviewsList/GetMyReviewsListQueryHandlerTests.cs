using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Review.Application.Queries.Me.GetMyReviewsList;
using Review.Domain.Entities;
using Review.Domain.Enums;
using Xunit;

namespace Review.Application.Tests.Queries.Me.GetMyReviewsList;

public class GetMyReviewsListQueryHandlerTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    public GetMyReviewsListQueryHandlerTests()
    {
        _currentUserService.Setup(x => x.UserId).Returns(_userId);
    }

    [Fact]
    public async Task Handle_WithOwnAndOtherUsersReviews_ReturnsOnlyOwnReviews()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var otherUserId = Guid.NewGuid();

        var mine = ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 4, "Mine");
        var notMine = ProductReview.Create(otherUserId, Guid.NewGuid(), ReviewItemType.Product, 2, "Not mine");
        dbContext.Reviews.AddRange(mine, notMine);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyReviewsListQueryHandler(dbContext, _currentUserService.Object);

        var result = await handler.Handle(new GetMyReviewsListQuery(1, 20), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().Id.Should().Be(mine.Id);
    }

    [Fact]
    public async Task Handle_WithMultipleReviews_OrdersByNewestFirst()
    {
        using var dbContext = TestReviewDbContextFactory.Create();

        var older = ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 3, "Older");
        await Task.Delay(10);
        var newer = ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 5, "Newer");
        dbContext.Reviews.AddRange(older, newer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyReviewsListQueryHandler(dbContext, _currentUserService.Object);

        var result = await handler.Handle(new GetMyReviewsListQuery(1, 20), CancellationToken.None);

        result.Items.First().Id.Should().Be(newer.Id);
        result.Items.Last().Id.Should().Be(older.Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsRequestedPageOnly()
    {
        using var dbContext = TestReviewDbContextFactory.Create();

        for (var i = 0; i < 5; i++)
        {
            dbContext.Reviews.Add(ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 3, $"Review {i}"));
            await Task.Delay(5);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyReviewsListQueryHandler(dbContext, _currentUserService.Object);

        var result = await handler.Handle(new GetMyReviewsListQuery(2, 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
}
