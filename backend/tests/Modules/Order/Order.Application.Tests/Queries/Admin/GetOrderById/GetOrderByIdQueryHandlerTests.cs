using FluentAssertions;
using Order.Application.Queries.Admin.GetOrderById;
using Order.Application.Tests.TestSupport;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.Application.Tests.Queries.Admin.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingOrder_ReturnsOrderDtoWithItems()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetOrderByIdQueryHandler(dbContext);

        var result = await handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        result.Id.Should().Be(order.Id);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var handler = new GetOrderByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
