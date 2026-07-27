using FluentAssertions;
using Inventory.Application.Queries.StockItems.GetStockItemById;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using Xunit;

namespace Inventory.Application.Tests.Queries.StockItems.GetStockItemById;

public class GetStockItemByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingStockItem_ReturnsDtoWithAvailableQuantity()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var stockItem = StockItem.Create(Guid.NewGuid(), InventoryItemType.Product, 10);
        var reservation = stockItem.Reserve(Guid.NewGuid(), 3, DateTime.UtcNow.AddMinutes(30));
        dbContext.StockItems.Add(stockItem);
        dbContext.StockReservations.Add(reservation);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStockItemByIdQueryHandler(dbContext);
        var result = await handler.Handle(new GetStockItemByIdQuery(stockItem.Id), CancellationToken.None);

        result.Id.Should().Be(stockItem.Id);
        result.QuantityOnHand.Should().Be(10);
        result.AvailableQuantity.Should().Be(7);
    }

    [Fact]
    public async Task Handle_WithNonExistentStockItem_ThrowsStockItemNotFoundException()
    {
        using var dbContext = TestInventoryDbContextFactory.Create();
        var handler = new GetStockItemByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<StockItemNotFoundException>(
            () => handler.Handle(new GetStockItemByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
