using FluentAssertions;
using Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;

namespace Inventory.Application.Tests.Commands.Reservations.ConfirmReservationsByReference;

public class ConfirmReservationsByReferenceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveReservations_ConfirmsThemAndDecreasesQuantityOnHand()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var referenceId = Guid.NewGuid();
        var reservation = stockItem.Reserve(referenceId, 4, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ConfirmReservationsByReferenceCommandHandler(dbContext);

        await handler.Handle(new ConfirmReservationsByReferenceCommand(referenceId), CancellationToken.None);

        var storedItem = await dbContext.StockItems.FindAsync(stockItem.Id);
        storedItem!.QuantityOnHand.Should().Be(6);
        var storedReservation = await dbContext.StockReservations.FindAsync(reservation.Id);
        storedReservation!.IsConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithUnknownReferenceId_ReturnsWithoutThrowing()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new ConfirmReservationsByReferenceCommandHandler(dbContext);

        await handler.Handle(new ConfirmReservationsByReferenceCommand(Guid.NewGuid()), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WithAllReservationsAlreadyConfirmed_ThrowsNoActiveReservationsForReferenceException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var referenceId = Guid.NewGuid();
        var reservation = stockItem.Reserve(referenceId, 4, DateTime.UtcNow.AddMinutes(30));
        stockItem.ConfirmReservation(reservation.Id);
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ConfirmReservationsByReferenceCommandHandler(dbContext);

        await Assert.ThrowsAsync<NoActiveReservationsForReferenceException>(
            () => handler.Handle(new ConfirmReservationsByReferenceCommand(referenceId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAllReservationsAlreadyReleased_ThrowsNoActiveReservationsForReferenceException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var referenceId = Guid.NewGuid();
        var reservation = stockItem.Reserve(referenceId, 4, DateTime.UtcNow.AddMinutes(30));
        stockItem.ReleaseReservation(reservation.Id);
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ConfirmReservationsByReferenceCommandHandler(dbContext);

        await Assert.ThrowsAsync<NoActiveReservationsForReferenceException>(
            () => handler.Handle(new ConfirmReservationsByReferenceCommand(referenceId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithMultipleActiveReservationsUnderSameReference_ConfirmsAllOfThem()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem1 = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var stockItem2 = StockItem.Create(Guid.NewGuid(), InventoryItemType.Variant, 10);
        var referenceId = Guid.NewGuid();
        var reservation1 = stockItem1.Reserve(referenceId, 2, DateTime.UtcNow.AddMinutes(30));
        var reservation2 = stockItem2.Reserve(referenceId, 3, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.AddRange(stockItem1, stockItem2);
        dbContext.StockReservations.AddRange(reservation1, reservation2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ConfirmReservationsByReferenceCommandHandler(dbContext);

        await handler.Handle(new ConfirmReservationsByReferenceCommand(referenceId), CancellationToken.None);

        (await dbContext.StockReservations.FindAsync(reservation1.Id))!.IsConfirmed.Should().BeTrue();
        (await dbContext.StockReservations.FindAsync(reservation2.Id))!.IsConfirmed.Should().BeTrue();
    }
}
