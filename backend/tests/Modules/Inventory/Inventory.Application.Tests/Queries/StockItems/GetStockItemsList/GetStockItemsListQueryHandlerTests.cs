using FluentAssertions;
using Inventory.Application.Queries.StockItems.GetStockItemsList;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Queries.StockItems.GetStockItemsList;

public class GetStockItemsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoFilter_ReturnsAllStockItemsPaged()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        dbContext.StockItems.AddRange(
            StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 5),
            StockItem.Create(Guid.NewGuid(), InventoryItemType.Variant, 10));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemsListQueryHandler(dbContext);
        var result = await handler.Handle(new GetStockItemsListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FilteredBySellableItemType_ReturnsOnlyMatchingItems()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        dbContext.StockItems.AddRange(
            StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 5),
            StockItem.Create(Guid.NewGuid(), InventoryItemType.Variant, 10));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemsListQueryHandler(dbContext);
        var result = await handler.Handle(
            new GetStockItemsListQuery(SellableItemType: InventoryItemType.Variant), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().SellableItemType.Should().Be(InventoryItemType.Variant);
    }

    [Fact]
    public async Task Handle_WithActiveReservation_ReflectsAvailableQuantityInDto()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var reservation = stockItem.Reserve(Guid.NewGuid(), 4, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemsListQueryHandler(dbContext);
        var result = await handler.Handle(new GetStockItemsListQuery(), CancellationToken.None);

        var dto = result.Items.Single();
        dto.QuantityOnHand.Should().Be(10);
        dto.AvailableQuantity.Should().Be(6);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPageSize()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        for (var i = 0; i < 5; i++)
            dbContext.StockItems.Add(StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, i));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemsListQueryHandler(dbContext);
        var result = await handler.Handle(new GetStockItemsListQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
    }
}
