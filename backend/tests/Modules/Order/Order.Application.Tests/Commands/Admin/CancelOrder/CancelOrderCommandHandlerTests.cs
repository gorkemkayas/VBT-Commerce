using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Order.Application.Commands.Admin.CancelOrder;
using Order.Application.Integrations;
using Order.Application.Tests.TestSupport;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.Application.Tests.Commands.Admin.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    private static Mock<ICurrentUserService> CreateCurrentUserService()
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.IpAddress).Returns("127.0.0.1");
        return mock;
    }

    [Fact]
    public async Task Handle_WithPendingOrder_ReleasesReservationAndCancelsOrder()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var inventory = new Mock<IInventoryIntegrationService>();
        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext, inventory: inventory);
        var handler = new CancelOrderCommandHandler(dbContext, CreateCurrentUserService().Object, orderOperations);

        await handler.Handle(new CancelOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        var stored = await dbContext.Orders.FindAsync(order.Id);
        stored!.Status.Should().Be(OrderStatus.Cancelled);
        stored.CancelledReason.Should().Be("Out of stock");
        inventory.Verify(x => x.ReleaseReservationsAsync(order.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithConfirmedOrder_RefundsPaymentAndCancelsOrder()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder();
        order.Confirm();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var payment = new Mock<IPaymentIntegrationService>();
        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext, payment: payment);
        var handler = new CancelOrderCommandHandler(dbContext, CreateCurrentUserService().Object, orderOperations);

        await handler.Handle(new CancelOrderCommand(order.Id, null), CancellationToken.None);

        var stored = await dbContext.Orders.FindAsync(order.Id);
        stored!.Status.Should().Be(OrderStatus.Cancelled);
        payment.Verify(x => x.RefundAsync(order.Id, "127.0.0.1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledOrder_ThrowsOrderInvalidStatusTransitionException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder();
        order.Cancel("first cancel");
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext);
        var handler = new CancelOrderCommandHandler(dbContext, CreateCurrentUserService().Object, orderOperations);

        await Assert.ThrowsAsync<OrderInvalidStatusTransitionException>(
            () => handler.Handle(new CancelOrderCommand(order.Id, "second cancel"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var orderOperations = OrderTestFactory.CreateOrderOperations(dbContext);
        var handler = new CancelOrderCommandHandler(dbContext, CreateCurrentUserService().Object, orderOperations);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new CancelOrderCommand(Guid.NewGuid(), null), CancellationToken.None));
    }
}
