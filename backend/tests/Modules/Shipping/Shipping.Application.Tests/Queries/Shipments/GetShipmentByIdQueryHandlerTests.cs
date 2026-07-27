using FluentAssertions;
using Shipping.Application.Queries.Shipments.GetShipmentById;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Queries.Shipments;

public class GetShipmentByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingShipment_ReturnsShipmentDtoWithHistory()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var shipment = Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShipmentByIdQueryHandler(dbContext);

        var result = await handler.Handle(new GetShipmentByIdQuery(shipment.Id), CancellationToken.None);

        result.Id.Should().Be(shipment.Id);
        result.OrderId.Should().Be(shipment.OrderId);
        result.History.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WithNonExistentShipment_ThrowsShipmentNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new GetShipmentByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<ShipmentNotFoundException>(
            () => handler.Handle(new GetShipmentByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
