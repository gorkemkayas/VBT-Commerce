using FluentAssertions;
using Shipping.Application.Queries.Shipments.GetShipmentByOrderId;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Queries.Shipments;

public class GetShipmentByOrderIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingOrder_ReturnsShipmentDto()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var orderId = Guid.NewGuid();
        var shipment = Shipment.Create(orderId, Guid.NewGuid());
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShipmentByOrderIdQueryHandler(dbContext);

        var result = await handler.Handle(new GetShipmentByOrderIdQuery(orderId), CancellationToken.None);

        result.Id.Should().Be(shipment.Id);
        result.OrderId.Should().Be(orderId);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ThrowsShipmentNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new GetShipmentByOrderIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<ShipmentNotFoundException>(
            () => handler.Handle(new GetShipmentByOrderIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
