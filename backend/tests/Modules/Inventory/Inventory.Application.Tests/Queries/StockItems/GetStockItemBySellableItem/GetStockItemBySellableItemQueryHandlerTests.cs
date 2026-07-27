using FluentAssertions;
using Inventory.Application.Queries.StockItems.GetStockItemBySellableItem;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;

namespace Inventory.Application.Tests.Queries.StockItems.GetStockItemBySellableItem;

public class GetStockItemBySellableItemQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingStockItem_ReturnsDto()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        var stockItem = StockItem.Create(sellableItemId, InventoryItemType.Variant, 8);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemBySellableItemQueryHandler(dbContext);
        var result = await handler.Handle(
            new GetStockItemBySellableItemQuery(sellableItemId, InventoryItemType.Variant), CancellationToken.None);

        result.Id.Should().Be(stockItem.Id);
        result.QuantityOnHand.Should().Be(8);
        result.AvailableQuantity.Should().Be(8);
    }

    [Fact]
    public async Task Handle_WithMismatchedSellableItemType_ThrowsStockItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        var stockItem = StockItem.Create(sellableItemId, InventoryItemType.Product, 8);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemBySellableItemQueryHandler(dbContext);

        await Assert.ThrowsAsync<StockItemNotFoundException>(
            () => handler.Handle(new GetStockItemBySellableItemQuery(sellableItemId, InventoryItemType.Variant), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentSellableItem_ThrowsStockItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new GetStockItemBySellableItemQueryHandler(dbContext);

        await Assert.ThrowsAsync<StockItemNotFoundException>(
            () => handler.Handle(new GetStockItemBySellableItemQuery(Guid.NewGuid(), InventoryItemType.Product), CancellationToken.None));
    }
}
