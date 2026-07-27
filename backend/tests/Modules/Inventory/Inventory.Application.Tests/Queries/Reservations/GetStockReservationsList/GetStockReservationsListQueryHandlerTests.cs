using FluentAssertions;
using Inventory.Application.Queries.Reservations.GetStockReservationsList;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Queries.Reservations.GetStockReservationsList;

public class GetStockReservationsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoFilters_ReturnsAllReservationsPaged()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 20);
        var reservation1 = stockItem.Reserve(Guid.NewGuid(), 2, DateTime.UtcNow.AddMinutes(30));
        var reservation2 = stockItem.Reserve(Guid.NewGuid(), 3, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.AddRange(reservation1, reservation2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockReservationsListQueryHandler(dbContext);
        var result = await handler.Handle(new GetStockReservationsListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FilteredBySellableItemId_ReturnsOnlyMatchingReservations()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem1 = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 20);
        var stockItem2 = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 20);
        var reservation1 = stockItem1.Reserve(Guid.NewGuid(), 2, DateTime.UtcNow.AddMinutes(30));
        var reservation2 = stockItem2.Reserve(Guid.NewGuid(), 3, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.AddRange(stockItem1, stockItem2);
        dbContext.StockReservations.AddRange(reservation1, reservation2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockReservationsListQueryHandler(dbContext);
        var result = await handler.Handle(
            new GetStockReservationsListQuery(SellableItemId: stockItem1.SellableItemId), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().StockItemId.Should().Be(stockItem1.Id);
    }

    [Fact]
    public async Task Handle_FilteredByIsConfirmed_ReturnsOnlyConfirmedReservations()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 20);
        var reservation1 = stockItem.Reserve(Guid.NewGuid(), 2, DateTime.UtcNow.AddMinutes(30));
        var reservation2 = stockItem.Reserve(Guid.NewGuid(), 3, DateTime.UtcNow.AddMinutes(30));
        stockItem.ConfirmReservation(reservation1.Id);
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.AddRange(reservation1, reservation2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockReservationsListQueryHandler(dbContext);
        var result = await handler.Handle(
            new GetStockReservationsListQuery(IsConfirmed: true), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().Id.Should().Be(reservation1.Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPageSize()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 100);
        for (var i = 0; i < 5; i++)
            dbContext.StockReservations.Add(stockItem.Reserve(Guid.NewGuid(), 1, DateTime.UtcNow.AddMinutes(30)));
        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockReservationsListQueryHandler(dbContext);
        var result = await handler.Handle(
            new GetStockReservationsListQuery(PageNumber: 1, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
}
