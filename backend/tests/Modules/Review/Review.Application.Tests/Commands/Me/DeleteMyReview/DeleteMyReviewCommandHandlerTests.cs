using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Review.Application.Commands.Me.DeleteMyReview;
using Review.Domain.Entities;
using Review.Domain.Enums;
using Review.Domain.Exceptions;
using Xunit;

namespace Review.Application.Tests.Commands.Me.DeleteMyReview;

public class DeleteMyReviewCommandHandlerTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    public DeleteMyReviewCommandHandlerTests()
    {
        _currentUserService.Setup(x => x.UserId).Returns(_userId);
    }

    [Fact]
    public async Task Handle_WithOwnedReview_DeletesReview()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var review = ProductReview.Create(_userId, Guid.NewGuid(), ReviewItemType.Product, 4, "Nice");
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await handler.Handle(new DeleteMyReviewCommand(review.Id), CancellationToken.None);

        var stored = await dbContext.Reviews.FindAsync(review.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentReview_ThrowsReviewNotFoundException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var handler = new DeleteMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await Assert.ThrowsAsync<ReviewNotFoundException>(
            () => handler.Handle(new DeleteMyReviewCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithReviewOwnedByAnotherUser_ThrowsReviewNotFoundException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var otherUserId = Guid.NewGuid();
        var review = ProductReview.Create(otherUserId, Guid.NewGuid(), ReviewItemType.Product, 4, "Not yours");
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteMyReviewCommandHandler(dbContext, _currentUserService.Object);

        await Assert.ThrowsAsync<ReviewNotFoundException>(
            () => handler.Handle(new DeleteMyReviewCommand(review.Id), CancellationToken.None));

        var stored = await dbContext.Reviews.FindAsync(review.Id);
        stored.Should().NotBeNull();
    }
}
