using FluentAssertions;
using Shipping.Application.Queries.Shipments.GetShipmentsList;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;
using Xunit;

namespace Shipping.Application.Tests.Queries.Shipments;

public class GetShipmentsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoStatusFilter_ReturnsAllShipmentsPaged()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var companyId = Guid.NewGuid();
        dbContext.Shipments.AddRange(
            Shipment.Create(Guid.NewGuid(), companyId),
            Shipment.Create(Guid.NewGuid(), companyId));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShipmentsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetShipmentsListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsOnlyMatchingShipments()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var companyId = Guid.NewGuid();
        var pending = Shipment.Create(Guid.NewGuid(), companyId);
        var shipped = Shipment.Create(Guid.NewGuid(), companyId);
        shipped.UpdateStatus(ShipmentStatus.Shipped, "TRK-1");
        dbContext.Shipments.AddRange(pending, shipped);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShipmentsListQueryHandler(dbContext);

        var result = await handler.Handle(
            new GetShipmentsListQuery(ShipmentStatus.Shipped), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().Id.Should().Be(shipped.Id);
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsRequestedPage()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var companyId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
            dbContext.Shipments.Add(Shipment.Create(Guid.NewGuid(), companyId));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShipmentsListQueryHandler(dbContext);

        var result = await handler.Handle(
            new GetShipmentsListQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
}
