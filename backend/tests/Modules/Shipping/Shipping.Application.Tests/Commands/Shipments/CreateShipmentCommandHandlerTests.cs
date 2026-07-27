using FluentAssertions;
using Shipping.Application.Commands.Shipments.CreateShipment;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Commands.Shipments;

public class CreateShipmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveShippingCompany_CreatesShipmentInPendingStatus()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var company = ShippingCompany.Create("Aras Kargo", 25m);
        dbContext.ShippingCompanies.Add(company);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateShipmentCommandHandler(dbContext);
        var orderId = Guid.NewGuid();
        var command = new CreateShipmentCommand(orderId, company.Id);

        var shipmentId = await handler.Handle(command, CancellationToken.None);

        shipmentId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Shipments.FindAsync(shipmentId);
        stored.Should().NotBeNull();
        stored!.OrderId.Should().Be(orderId);
        stored.ShippingCompanyId.Should().Be(company.Id);
        stored.Status.Should().Be(ShipmentStatus.Pending);
    }

    [Fact]
    public async Task Handle_WithNonExistentShippingCompany_ThrowsShippingCompanyNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new CreateShipmentCommandHandler(dbContext);
        var command = new CreateShipmentCommand(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<ShippingCompanyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInactiveShippingCompany_ThrowsShippingCompanyInactiveException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var company = ShippingCompany.Create("Yurtiçi Kargo", 20m);
        company.Deactivate();
        dbContext.ShippingCompanies.Add(company);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateShipmentCommandHandler(dbContext);
        var command = new CreateShipmentCommand(Guid.NewGuid(), company.Id);

        await Assert.ThrowsAsync<ShippingCompanyInactiveException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
