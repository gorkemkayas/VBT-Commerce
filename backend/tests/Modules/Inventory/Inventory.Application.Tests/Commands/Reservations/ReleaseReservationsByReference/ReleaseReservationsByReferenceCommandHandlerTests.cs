using FluentAssertions;
using Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventory.Application.Tests.Commands.Reservations.ReleaseReservationsByReference;

public class ReleaseReservationsByReferenceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveReservations_ReleasesThemWithoutChangingQuantityOnHand()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var referenceId = Guid.NewGuid();
        var reservation = stockItem.Reserve(referenceId, 4, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ReleaseReservationsByReferenceCommandHandler(dbContext);

        await handler.Handle(new ReleaseReservationsByReferenceCommand(referenceId), CancellationToken.None);

        var storedItem = await dbContext.StockItems.FindAsync(stockItem.Id);
        storedItem!.QuantityOnHand.Should().Be(10);
        var storedReservation = await dbContext.StockReservations.FindAsync(reservation.Id);
        storedReservation!.IsReleased.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithUnknownReferenceId_ReturnsWithoutThrowing()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new ReleaseReservationsByReferenceCommandHandler(dbContext);

        await handler.Handle(new ReleaseReservationsByReferenceCommand(Guid.NewGuid()), CancellationToken.None);
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

        var handler = new ReleaseReservationsByReferenceCommandHandler(dbContext);

        await Assert.ThrowsAsync<NoActiveReservationsForReferenceException>(
            () => handler.Handle(new ReleaseReservationsByReferenceCommand(referenceId), CancellationToken.None));
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

        var handler = new ReleaseReservationsByReferenceCommandHandler(dbContext);

        await Assert.ThrowsAsync<NoActiveReservationsForReferenceException>(
            () => handler.Handle(new ReleaseReservationsByReferenceCommand(referenceId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AfterRelease_FreesUpAvailableQuantityForNewReservations()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var referenceId = Guid.NewGuid();
        var reservation = stockItem.Reserve(referenceId, 10, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ReleaseReservationsByReferenceCommandHandler(dbContext);
        await handler.Handle(new ReleaseReservationsByReferenceCommand(referenceId), CancellationToken.None);

        var storedItem = await dbContext.StockItems.Include(s => s.Reservations).FirstAsync(s => s.Id == stockItem.Id);
        storedItem.AvailableQuantity(DateTime.UtcNow).Should().Be(10);
    }
}
