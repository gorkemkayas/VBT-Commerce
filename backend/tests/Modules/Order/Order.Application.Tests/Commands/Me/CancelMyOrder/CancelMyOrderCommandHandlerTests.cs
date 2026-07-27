using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Order.Application.Commands.Me.CancelMyOrder;
using Order.Application.Integrations;
using Order.Application.Tests.TestSupport;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.Application.Tests.Commands.Me.CancelMyOrder;

public class CancelMyOrderCommandHandlerTests
{
    private static Mock<ICurrentUserService> CreateCurrentUserService(Guid userId)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        mock.Setup(x => x.IpAddress).Returns("127.0.0.1");
        return mock;
    }

    [Fact]
    public async Task Handle_WithOwnedPendingOrder_ReleasesReservationAndCancelsOrder()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var order = OrderTestFactory.CreatePendingOrder(userId: userId);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var inventory = new Mock<IInventoryIntegrationService>();
        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext, inventory: inventory);
        var handler = new CancelMyOrderCommandHandler(dbContext, CreateCurrentUserService(userId).Object, orderOperations);

        await handler.Handle(new CancelMyOrderCommand(order.Id), CancellationToken.None);

        var stored = await dbContext.Orders.FindAsync(order.Id);
        stored!.Status.Should().Be(OrderStatus.Cancelled);
        stored.CancelledReason.Should().Be("Cancelled by customer");
        inventory.Verify(x => x.ReleaseReservationsAsync(order.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithOrderBelongingToAnotherUser_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder(userId: Guid.NewGuid());
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext);
        var handler = new CancelMyOrderCommandHandler(dbContext, CreateCurrentUserService(Guid.NewGuid()).Object, orderOperations);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new CancelMyOrderCommand(order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext);
        var handler = new CancelMyOrderCommandHandler(dbContext, CreateCurrentUserService(Guid.NewGuid()).Object, orderOperations);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new CancelMyOrderCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledOrder_ThrowsOrderInvalidStatusTransitionException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var order = OrderTestFactory.CreatePendingOrder(userId: userId);
        order.Cancel("already cancelled");
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext);
        var handler = new CancelMyOrderCommandHandler(dbContext, CreateCurrentUserService(userId).Object, orderOperations);

        await Assert.ThrowsAsync<OrderInvalidStatusTransitionException>(
            () => handler.Handle(new CancelMyOrderCommand(order.Id), CancellationToken.None));
    }
}
