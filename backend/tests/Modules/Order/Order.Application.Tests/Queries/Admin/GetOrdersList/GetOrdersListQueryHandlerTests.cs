using FluentAssertions;
using Order.Application.Queries.Admin.GetOrdersList;
using Order.Application.Tests.TestSupport;
using Order.Domain.Enums;
using Xunit;

namespace Order.Application.Tests.Queries.Admin.GetOrdersList;

public class GetOrdersListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoStatusFilter_ReturnsAllOrdersPaged()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var pending = OrderTestFactory.CreatePendingOrder();
        var confirmed = OrderTestFactory.CreatePendingOrder();
        confirmed.Confirm();
        dbContext.Orders.AddRange(pending, confirmed);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetOrdersListQueryHandler(dbContext);

        var result = await handler.Handle(new GetOrdersListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsOnlyMatchingOrders()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var pending = OrderTestFactory.CreatePendingOrder();
        var confirmed = OrderTestFactory.CreatePendingOrder();
        confirmed.Confirm();
        dbContext.Orders.AddRange(pending, confirmed);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetOrdersListQueryHandler(dbContext);

        var result = await handler.Handle(new GetOrdersListQuery(OrderStatus.Confirmed), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().Id.Should().Be(confirmed.Id);
    }

    [Fact]
    public async Task Handle_WithPageSizeSmallerThanTotal_ReturnsCorrectPageAndTotalCount()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        for (var i = 0; i < 3; i++)
            dbContext.Orders.Add(OrderTestFactory.CreatePendingOrder());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetOrdersListQueryHandler(dbContext);

        var result = await handler.Handle(new GetOrdersListQuery(PageNumber: 1, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(2);
    }
}
