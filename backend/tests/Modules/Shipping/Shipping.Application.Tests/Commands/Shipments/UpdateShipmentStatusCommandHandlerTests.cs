using FluentAssertions;
using Shipping.Application.Commands.Shipments.UpdateShipmentStatus;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Commands.Shipments;

public class UpdateShipmentStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidTransition_UpdatesStatusAndTrackingNumber()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var shipment = Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateShipmentStatusCommandHandler(dbContext);
        var command = new UpdateShipmentStatusCommand(shipment.Id, ShipmentStatus.Shipped, "TRK-123");

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Shipments.FindAsync(shipment.Id);
        stored!.Status.Should().Be(ShipmentStatus.Shipped);
        stored.TrackingNumber.Should().Be("TRK-123");
        stored.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_RecordsNewStatusHistoryEntry()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var shipment = Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateShipmentStatusCommandHandler(dbContext);
        var command = new UpdateShipmentStatusCommand(shipment.Id, ShipmentStatus.InTransit, null);

        await handler.Handle(command, CancellationToken.None);

        var historyCount = dbContext.ShipmentStatusHistories.Count(h => h.ShipmentId == shipment.Id);
        historyCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithNonExistentShipment_ThrowsShipmentNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new UpdateShipmentStatusCommandHandler(dbContext);
        var command = new UpdateShipmentStatusCommand(Guid.NewGuid(), ShipmentStatus.Shipped, null);

        await Assert.ThrowsAsync<ShipmentNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyDeliveredShipment_ThrowsShipmentAlreadyFinalizedException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var shipment = Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        shipment.UpdateStatus(ShipmentStatus.Delivered, null);
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateShipmentStatusCommandHandler(dbContext);
        var command = new UpdateShipmentStatusCommand(shipment.Id, ShipmentStatus.InTransit, null);

        await Assert.ThrowsAsync<ShipmentAlreadyFinalizedException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledShipment_ThrowsShipmentAlreadyFinalizedException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var shipment = Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        shipment.UpdateStatus(ShipmentStatus.Cancelled, null);
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateShipmentStatusCommandHandler(dbContext);
        var command = new UpdateShipmentStatusCommand(shipment.Id, ShipmentStatus.Delivered, null);

        await Assert.ThrowsAsync<ShipmentAlreadyFinalizedException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
