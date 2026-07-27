using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Order.Application.Queries.Me.GetMyOrderById;
using Order.Application.Tests.TestSupport;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.Application.Tests.Queries.Me.GetMyOrderById;

public class GetMyOrderByIdQueryHandlerTests
{
    private static Mock<ICurrentUserService> CreateCurrentUserService(Guid userId)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        return mock;
    }

    [Fact]
    public async Task Handle_WithOwnedOrder_ReturnsOrderDto()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var order = OrderTestFactory.CreatePendingOrder(userId: userId);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyOrderByIdQueryHandler(dbContext, CreateCurrentUserService(userId).Object);

        var result = await handler.Handle(new GetMyOrderByIdQuery(order.Id), CancellationToken.None);

        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task Handle_WithOrderBelongingToAnotherUser_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder(userId: Guid.NewGuid());
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetMyOrderByIdQueryHandler(dbContext, CreateCurrentUserService(Guid.NewGuid()).Object);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new GetMyOrderByIdQuery(order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var handler = new GetMyOrderByIdQueryHandler(dbContext, CreateCurrentUserService(Guid.NewGuid()).Object);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new GetMyOrderByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
