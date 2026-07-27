using FluentAssertions;
using Inventory.Application.Commands.StockItems.IncreaseStock;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;

namespace Inventory.Application.Tests.Commands.StockItems.IncreaseStock;

public class IncreaseStockCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingStockItem_IncreasesStock()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new IncreaseStockCommandHandler(dbContext);
        await handler.Handle(new IncreaseStockCommand(stockItem.Id, 5), CancellationToken.None);

        var stored = await dbContext.StockItems.FindAsync(stockItem.Id);
        stored!.QuantityOnHand.Should().Be(15);
    }

    [Fact]
    public async Task Handle_WithNonExistentStockItem_ThrowsStockItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new IncreaseStockCommandHandler(dbContext);

        await Assert.ThrowsAsync<StockItemNotFoundException>(
            () => handler.Handle(new IncreaseStockCommand(Guid.NewGuid(), 5), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithZeroQuantity_ThrowsInvalidStockQuantityException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new IncreaseStockCommandHandler(dbContext);

        await Assert.ThrowsAsync<InvalidStockQuantityException>(
            () => handler.Handle(new IncreaseStockCommand(stockItem.Id, 0), CancellationToken.None));
    }
}
