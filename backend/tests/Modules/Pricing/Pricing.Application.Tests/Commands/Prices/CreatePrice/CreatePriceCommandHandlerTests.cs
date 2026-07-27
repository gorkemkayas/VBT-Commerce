using FluentAssertions;
using Moq;
using Pricing.Application.Commands.Prices.CreatePrice;
using Pricing.Application.Integrations;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Commands.Prices.CreatePrice;

public class CreatePriceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSellableItemAndNoPrice_CreatesPrice()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        catalogIntegrationServiceMock
            .Setup(s => s.SellableItemExistsAsync(It.IsAny<Guid>(), It.IsAny<PriceItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreatePriceCommandHandler(dbContext, catalogIntegrationServiceMock.Object);
        var sellableItemId = Guid.NewGuid();
        var command = new CreatePriceCommand(sellableItemId, PriceItemType.Product, 19.99m);

        var priceId = await handler.Handle(command, CancellationToken.None);

        priceId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Prices.FindAsync(priceId);
        stored!.Amount.Should().Be(19.99m);
        stored.SellableItemId.Should().Be(sellableItemId);
    }

    [Fact]
    public async Task Handle_WithNonExistentSellableItem_ThrowsPricingSellableItemNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        catalogIntegrationServiceMock
            .Setup(s => s.SellableItemExistsAsync(It.IsAny<Guid>(), It.IsAny<PriceItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CreatePriceCommandHandler(dbContext, catalogIntegrationServiceMock.Object);
        var command = new CreatePriceCommand(Guid.NewGuid(), PriceItemType.Product, 19.99m);

        await Assert.ThrowsAsync<PricingSellableItemNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithPriceAlreadyExisting_ThrowsPriceAlreadyExistsException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        dbContext.Prices.Add(Price.Create(sellableItemId, PriceItemType.Product, 10m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        catalogIntegrationServiceMock
            .Setup(s => s.SellableItemExistsAsync(It.IsAny<Guid>(), It.IsAny<PriceItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreatePriceCommandHandler(dbContext, catalogIntegrationServiceMock.Object);
        var command = new CreatePriceCommand(sellableItemId, PriceItemType.Product, 19.99m);

        await Assert.ThrowsAsync<PriceAlreadyExistsException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
