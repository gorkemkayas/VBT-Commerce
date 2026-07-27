using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Order.Application.Integrations;
using Order.Application.Queries.Me.GetMyOrderShipment;
using Order.Application.Tests.TestSupport;
using Order.Domain.Exceptions;
using Shipping.Contracts;
using Shipping.Domain.Enums;
using Xunit;

namespace Order.Application.Tests.Queries.Me.GetMyOrderShipment;

public class GetMyOrderShipmentQueryHandlerTests
{
    private static Mock<ICurrentUserService> CreateCurrentUserService(Guid userId)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        return mock;
    }

    [Fact]
    public async Task Handle_WithOwnedOrderAndExistingShipment_ReturnsShipmentTrackingDto()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var order = OrderTestFactory.CreatePendingOrder(userId: userId);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var shipmentDto = new ShipmentTrackingDto(Guid.NewGuid(), ShipmentStatus.Pending, null, DateTime.UtcNow, null, []);
        var shipping = new Mock<IShippingIntegrationService>();
        shipping.Setup(x => x.GetShipmentByOrderIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipmentDto);

        var handler = new GetMyOrderShipmentQueryHandler(dbContext, CreateCurrentUserService(userId).Object, shipping.Object);

        var result = await handler.Handle(new GetMyOrderShipmentQuery(order.Id), CancellationToken.None);

        result.Should().Be(shipmentDto);
    }

    [Fact]
    public async Task Handle_WithOrderBelongingToAnotherUser_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder(userId: Guid.NewGuid());
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var shipping = new Mock<IShippingIntegrationService>();
        var handler = new GetMyOrderShipmentQueryHandler(dbContext, CreateCurrentUserService(Guid.NewGuid()).Object, shipping.Object);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new GetMyOrderShipmentQuery(order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNoShipmentForOrder_ThrowsOrderShipmentNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var order = OrderTestFactory.CreatePendingOrder(userId: userId);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var shipping = new Mock<IShippingIntegrationService>();
        shipping.Setup(x => x.GetShipmentByOrderIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync((ShipmentTrackingDto?)null);

        var handler = new GetMyOrderShipmentQueryHandler(dbContext, CreateCurrentUserService(userId).Object, shipping.Object);

        await Assert.ThrowsAsync<OrderShipmentNotFoundException>(
            () => handler.Handle(new GetMyOrderShipmentQuery(order.Id), CancellationToken.None));
    }
}
