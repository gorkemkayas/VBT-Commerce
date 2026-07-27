using FluentAssertions;
using Inventory.Application.Queries.Reservations.GetAvailableQuantity;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Queries.Reservations.GetAvailableQuantity;

public class GetAvailableQuantityQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoActiveReservations_ReturnsFullQuantityOnHand()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        var stockItem = StockItem.Create(sellableItemId, InventoryItemType.Product, 10);
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAvailableQuantityQueryHandler(dbContext);
        var result = await handler.Handle(new GetAvailableQuantityQuery(sellableItemId, InventoryItemType.Product), CancellationToken.None);

        result.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithActiveReservation_SubtractsReservedQuantity()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        var stockItem = StockItem.Create(sellableItemId, InventoryItemType.Product, 10);
        var reservation = stockItem.Reserve(Guid.NewGuid(), 4, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAvailableQuantityQueryHandler(dbContext);
        var result = await handler.Handle(new GetAvailableQuantityQuery(sellableItemId, InventoryItemType.Product), CancellationToken.None);

        result.Should().Be(6);
    }

    [Fact]
    public async Task Handle_WithExpiredReservation_DoesNotSubtractExpiredQuantity()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        var stockItem = StockItem.Create(sellableItemId, InventoryItemType.Product, 10);
        // Reserve() itself only checks *currently active* reservations for availability, so an
        // already-expired window can still be created directly via the expiresAt parameter.
        var reservation = stockItem.Reserve(Guid.NewGuid(), 4, DateTime.UtcNow.AddMinutes(-1));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAvailableQuantityQueryHandler(dbContext);
        var result = await handler.Handle(new GetAvailableQuantityQuery(sellableItemId, InventoryItemType.Product), CancellationToken.None);

        result.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithNoStockItemTracked_ReturnsZero()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new GetAvailableQuantityQueryHandler(dbContext);

        var result = await handler.Handle(new GetAvailableQuantityQuery(Guid.NewGuid(), InventoryItemType.Product), CancellationToken.None);

        result.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithConfirmedReservation_DoesNotSubtractConfirmedQuantity()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var sellableItemId = Guid.NewGuid();
        var stockItem = StockItem.Create(sellableItemId, InventoryItemType.Product, 10);
        var reservation = stockItem.Reserve(Guid.NewGuid(), 4, DateTime.UtcNow.AddMinutes(30));
        stockItem.ConfirmReservation(reservation.Id);
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAvailableQuantityQueryHandler(dbContext);
        var result = await handler.Handle(new GetAvailableQuantityQuery(sellableItemId, InventoryItemType.Product), CancellationToken.None);

        // Confirming already deducted the quantity from QuantityOnHand (10 - 4 = 6), and a confirmed
        // reservation is no longer "active" so it isn't subtracted a second time.
        result.Should().Be(6);
    }
}
