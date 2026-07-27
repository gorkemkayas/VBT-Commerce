using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Review.Application.Commands.Me.CreateMyReview;
using Review.Application.Integrations;
using Review.Domain.Enums;
using Review.Domain.Exceptions;
using Xunit;

namespace Review.Application.Tests.Commands.Me.CreateMyReview;

public class CreateMyReviewCommandHandlerTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<ICatalogIntegrationService> _catalogIntegrationService = new();
    private readonly Mock<IOrderIntegrationService> _orderIntegrationService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    public CreateMyReviewCommandHandlerTests()
    {
        _currentUserService.Setup(x => x.UserId).Returns(_userId);
    }

    private CreateMyReviewCommandHandler CreateHandler(Abstractions.IReviewDbContext dbContext) =>
        new(dbContext, _catalogIntegrationService.Object, _orderIntegrationService.Object, _currentUserService.Object);

    [Fact]
    public async Task Handle_WithValidPurchaseAndNoExistingReview_CreatesReview()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        _catalogIntegrationService.Setup(x => x.SellableItemExistsAsync(itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _orderIntegrationService.Setup(x => x.HasPurchasedItemAsync(_userId, itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler(dbContext);
        var command = new CreateMyReviewCommand(itemId, ReviewItemType.Product, 5, "Great product");

        var reviewId = await handler.Handle(command, CancellationToken.None);

        reviewId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Reviews.FindAsync(reviewId);
        stored.Should().NotBeNull();
        stored!.UserId.Should().Be(_userId);
        stored.Rating.Should().Be(5);
        stored.Comment.Should().Be("Great product");
    }

    [Fact]
    public async Task Handle_WithNonExistentSellableItem_ThrowsReviewSellableItemNotFoundException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        _catalogIntegrationService.Setup(x => x.SellableItemExistsAsync(itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler(dbContext);
        var command = new CreateMyReviewCommand(itemId, ReviewItemType.Product, 5, "Great product");

        await Assert.ThrowsAsync<ReviewSellableItemNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserHasNotPurchasedItem_ThrowsForbiddenException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();

        _catalogIntegrationService.Setup(x => x.SellableItemExistsAsync(itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _orderIntegrationService.Setup(x => x.HasPurchasedItemAsync(_userId, itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _orderIntegrationService.Setup(x => x.GetPurchasedVariantIdsAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());

        var handler = CreateHandler(dbContext);
        var command = new CreateMyReviewCommand(itemId, ReviewItemType.Product, 5, "Great product");

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserPurchasedVariantOfProduct_CreatesReview()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        _catalogIntegrationService.Setup(x => x.SellableItemExistsAsync(productId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _orderIntegrationService.Setup(x => x.HasPurchasedItemAsync(_userId, productId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _orderIntegrationService.Setup(x => x.GetPurchasedVariantIdsAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([variantId]);
        _catalogIntegrationService.Setup(x => x.GetVariantProductIdAsync(variantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productId);

        var handler = CreateHandler(dbContext);
        var command = new CreateMyReviewCommand(productId, ReviewItemType.Product, 4, "Bought a variant");

        var reviewId = await handler.Handle(command, CancellationToken.None);

        reviewId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyReviewedItem_ThrowsDuplicateReviewException()
    {
        using var dbContext = TestReviewDbContextFactory.Create();
        var itemId = Guid.NewGuid();
        dbContext.Reviews.Add(Domain.Entities.ProductReview.Create(_userId, itemId, ReviewItemType.Product, 3, "Already reviewed"));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        _catalogIntegrationService.Setup(x => x.SellableItemExistsAsync(itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _orderIntegrationService.Setup(x => x.HasPurchasedItemAsync(_userId, itemId, ReviewItemType.Product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler(dbContext);
        var command = new CreateMyReviewCommand(itemId, ReviewItemType.Product, 5, "Second review attempt");

        await Assert.ThrowsAsync<DuplicateReviewException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
