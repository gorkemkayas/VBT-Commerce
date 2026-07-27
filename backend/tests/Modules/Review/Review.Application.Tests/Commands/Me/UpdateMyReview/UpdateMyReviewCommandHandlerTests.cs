using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Review.Application.Commands.Me.UpdateMyReview;
using Review.Domain.Entities;
using Review.Domain.Enums;
using Review.Domain.Exceptions;
using Xunit;

namespace Review.Application.Tests.Commands.Me.UpdateMyReview;

public class UpdateMyReviewCommandHandlerTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    public UpdateMyReviewCommandHandlerTests()
    {
        _currentUserService.Setup(x => x.UserId).Returns(_userId);
    }

    [Fact]
    public async Task Handle_WithOwnedReview_UpdatesRatingAndComment()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var review = ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 3, "Ok product");
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await handler.Handle(new UpdateMyReviewCommand(review.Id, 5, "Actually great"), CancellationToken.None);

        var stored = await dbContext.Reviews.FindAsync(review.Id);
        stored!.Rating.Should().Be(5);
        stored.Comment.Should().Be("Actually great");
        stored.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentReview_ThrowsReviewNotFoundException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var handler = new UpdateMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await Assert.ThrowsAsync<ReviewNotFoundException>(
            () => handler.Handle(new UpdateMyReviewCommand(Guid.NewGuid(), 4, "Update"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithReviewOwnedByAnotherUser_ThrowsReviewNotFoundException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var otherUserId = Guid.NewGuid();
        var review = ProductReview.Create(otherUserId, Guid.NewGuid(), ReviewItemType.Product, 3, "Not yours");
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await Assert.ThrowsAsync<ReviewNotFoundException>(
            () => handler.Handle(new UpdateMyReviewCommand(review.Id, 5, "Hijack attempt"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidRating_ThrowsInvalidRatingException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var review = ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 3, "Ok product");
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await Assert.ThrowsAsync<InvalidRatingException>(
            () => handler.Handle(new UpdateMyReviewCommand(review.Id, 7, "Bad rating"), CancellationToken.None));
    }
}
