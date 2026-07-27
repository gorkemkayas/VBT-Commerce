using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Order.Application.Queries.Me.GetMyOrdersList;
using Order.Application.Tests.TestSupport;
using Xunit;

namespace Order.Application.Tests.Queries.Me.GetMyOrdersList;

public class GetMyOrdersListQueryHandlerTests
{
    private static Mock<ICurrentUserService> CreateCurrentUserService(Guid userId)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        return mock;
    }

    [Fact]
    public async Task Handle_ReturnsOnlyOrdersOwnedByCurrentUser()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var myOrder = OrderTestFactory.CreatePendingOrder(userId: userId);
        var otherOrder = OrderTestFactory.CreatePendingOrder(userId: Guid.NewGuid());
        dbContext.Orders.AddRange(myOrder, otherOrder);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyOrdersListQueryHandler(dbContext, CreateCurrentUserService(userId).Object);

        var result = await handler.Handle(new GetMyOrdersListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().Id.Should().Be(myOrder.Id);
    }

    [Fact]
    public async Task Handle_WithNoOrders_ReturnsEmptyPagedResult()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var handler = new GetMyOrdersListQueryHandler(dbContext, CreateCurrentUserService(Guid.NewGuid()).Object);

        var result = await handler.Handle(new GetMyOrdersListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithPageSizeSmallerThanTotal_ReturnsCorrectPage()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 3; i++)
            dbContext.Orders.Add(OrderTestFactory.CreatePendingOrder(userId: userId));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyOrdersListQueryHandler(dbContext, CreateCurrentUserService(userId).Object);

        var result = await handler.Handle(new GetMyOrdersListQuery(PageNumber: 1, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(2);
    }
}
