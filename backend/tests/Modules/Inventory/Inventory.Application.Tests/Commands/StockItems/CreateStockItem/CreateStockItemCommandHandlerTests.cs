using FluentAssertions;
using Inventory.Application.Commands.StockItems.CreateStockItem;
using Inventory.Application.Integrations;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Moq;
using Xunit;

namespace Inventory.Application.Tests.Commands.StockItems.CreateStockItem;

public class CreateStockItemCommandHandlerTests
{
    private static Mock<ICatalogIntegrationService> CreateCatalogIntegrationServiceMock(bool sellableItemExists = true)
    {
        var mock = new Mock<ICatalogIntegrationService>();
        mock.Setup(s => s.SellableItemExistsAsync(It.IsAny<Guid>(), It.IsAny<InventoryItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sellableItemExists);
        return mock;
    }

    [Fact]
    public async Task Handle_WithExistingSellableItemAndNoDuplicate_CreatesStockItem()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var catalogIntegrationService = CreateCatalogIntegrationServiceMock();
        var handler = new CreateStockItemCommandHandler(dbContext, catalogIntegrationService.Object);
        var sellableItemId = Guid.NewGuid();
        var command = new CreateStockItemCommand(sellableItemId, InventoryItemType.Product, 20);

        var stockItemId = await handler.Handle(command, CancellationToken.None);

        stockItemId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.StockItems.FindAsync(stockItemId);
        stored.Should().NotBeNull();
        stored!.SellableItemId.Should().Be(sellableItemId);
        stored.QuantityOnHand.Should().Be(20);
    }

    [Fact]
    public async Task Handle_WithNonExistentSellableItem_ThrowsSellableItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var catalogIntegrationService = CreateCatalogIntegrationServiceMock(sellableItemExists: false);
        var handler = new CreateStockItemCommandHandler(dbContext, catalogIntegrationService.Object);
        var command = new CreateStockItemCommand(Guid.NewGuid(), InventoryItemType.Product, 20);

        await Assert.ThrowsAsync<SellableItemNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateStockItemForSameSellableItem_ThrowsDuplicateStockItemException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var catalogIntegrationService = CreateCatalogIntegrationServiceMock();
        var sellableItemId = Guid.NewGuid();
        var existing = Domain.Entities.StockItem.Create(sellableItemId, InventoryItemType.Product, 5);
        dbContext.StockItems.Add(existing);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateStockItemCommandHandler(dbContext, catalogIntegrationService.Object);
        var command = new CreateStockItemCommand(sellableItemId, InventoryItemType.Product, 10);

        await Assert.ThrowsAsync<DuplicateStockItemException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSameSellableItemIdButDifferentType_CreatesSeparateStockItem()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var catalogIntegrationService = CreateCatalogIntegrationServiceMock();
        var sellableItemId = Guid.NewGuid();
        var existing = Domain.Entities.StockItem.Create(sellableItemId, InventoryItemType.Product, 5);
        dbContext.StockItems.Add(existing);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateStockItemCommandHandler(dbContext, catalogIntegrationService.Object);
        var command = new CreateStockItemCommand(sellableItemId, InventoryItemType.Variant, 10);

        var stockItemId = await handler.Handle(command, CancellationToken.None);

        stockItemId.Should().NotBe(existing.Id);
    }
}
