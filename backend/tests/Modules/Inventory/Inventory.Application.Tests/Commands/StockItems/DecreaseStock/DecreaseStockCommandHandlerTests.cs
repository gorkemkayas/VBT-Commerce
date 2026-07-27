using FluentAssertions;
using Inventory.Application.Commands.StockItems.DecreaseStock;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;

namespace Inventory.Application.Tests.Commands.StockItems.DecreaseStock;

public class DecreaseStockCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithSufficientQuantityOnHand_DecreasesStock()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DecreaseStockCommandHandler(dbContext);
        await handler.Handle(new DecreaseStockCommand(stockItem.Id, 4), CancellationToken.None);

        var stored = await dbContext.StockItems.FindAsync(stockItem.Id);
        stored!.QuantityOnHand.Should().Be(6);
    }

    [Fact]
    public async Task Handle_WithNonExistentStockItem_ThrowsStockItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new DecreaseStockCommandHandler(dbContext);

        await Assert.ThrowsAsync<StockItemNotFoundException>(
            () => handler.Handle(new DecreaseStockCommand(Guid.NewGuid(), 4), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithQuantityExceedingOnHand_ThrowsInvalidStockQuantityException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 5);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DecreaseStockCommandHandler(dbContext);

        await Assert.ThrowsAsync<InvalidStockQuantityException>(
            () => handler.Handle(new DecreaseStockCommand(stockItem.Id, 6), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DecreasingToExactlyZero_Succeeds()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 5);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DecreaseStockCommandHandler(dbContext);
        await handler.Handle(new DecreaseStockCommand(stockItem.Id, 5), CancellationToken.None);

        var stored = await dbContext.StockItems.FindAsync(stockItem.Id);
        stored!.QuantityOnHand.Should().Be(0);
    }
}
