using FluentAssertions;
using Inventory.Application.Commands.Reservations.ReserveStock;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;

namespace Inventory.Application.Tests.Commands.Reservations.ReserveStock;

public class ReserveStockCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithSufficientStock_CreatesReservation()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ReserveStockCommandHandler(dbContext);
        var referenceId = Guid.NewGuid();
        var command = new ReserveStockCommand(referenceId, [new ReserveStockLineItem(stockItem.SellableItemId, InventoryItemType.Product, 4)]);

        await handler.Handle(command, CancellationToken.None);

        var reservations = dbContext.StockReservations.ToList();
        reservations.Should().ContainSingle();
        reservations[0].ReferenceId.Should().Be(referenceId);
        reservations[0].Quantity.Should().Be(4);
        reservations[0].StockItemId.Should().Be(stockItem.Id);
    }

    [Fact]
    public async Task Handle_WithMultipleLineItems_CreatesAllReservations()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem1 = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var stockItem2 = StockItem.Create(Guid.NewGuid(), InventoryItemType.Variant, 5);
        dbContext.StockItems.AddRange(stockItem1, stockItem2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ReserveStockCommandHandler(dbContext);
        var command = new ReserveStockCommand(Guid.NewGuid(), [
            new ReserveStockLineItem(stockItem1.SellableItemId, InventoryItemType.Product, 2),
            new ReserveStockLineItem(stockItem2.SellableItemId, InventoryItemType.Variant, 3)
        ]);

        await handler.Handle(command, CancellationToken.None);

        dbContext.StockReservations.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNonExistentStockItem_ThrowsStockItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new ReserveStockCommandHandler(dbContext);
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(Guid.NewGuid(), InventoryItemType.Product, 1)]);

        await Assert.ThrowsAsync<StockItemNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithQuantityExceedingAvailable_ThrowsInsufficientStockException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 5);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ReserveStockCommandHandler(dbContext);
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(stockItem.SellableItemId, InventoryItemType.Product, 6)]);

        await Assert.ThrowsAsync<InsufficientStockException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyReservedQuantity_ThrowsInsufficientStockExceptionForRemainder()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var existingReservation = stockItem.Reserve(Guid.NewGuid(), 8, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(existingReservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ReserveStockCommandHandler(dbContext);
        var command = new ReserveStockCommand(Guid.NewGuid(), [new ReserveStockLineItem(stockItem.SellableItemId, InventoryItemType.Product, 3)]);

        await Assert.ThrowsAsync<InsufficientStockException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
