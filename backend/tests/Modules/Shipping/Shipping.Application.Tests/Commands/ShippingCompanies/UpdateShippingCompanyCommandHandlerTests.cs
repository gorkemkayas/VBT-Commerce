using FluentAssertions;
using Shipping.Application.Commands.ShippingCompanies.UpdateShippingCompany;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Commands.ShippingCompanies;

public class UpdateShippingCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_UpdatesShippingCompany()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var company = ShippingCompany.Create("Aras Kargo", 25m);
        dbContext.ShippingCompanies.Add(company);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateShippingCompanyCommandHandler(dbContext);
        var command = new UpdateShippingCompanyCommand(company.Id, "Aras Kargo Updated", 30m);

        await handler.Handle(command, CancellationToken.None);

        var updated = await dbContext.ShippingCompanies.FindAsync(company.Id);
        updated!.Name.Should().Be("Aras Kargo Updated");
        updated.Fee.Should().Be(30m);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentCompany_ThrowsShippingCompanyNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new UpdateShippingCompanyCommandHandler(dbContext);
        var command = new UpdateShippingCompanyCommand(Guid.NewGuid(), "Aras Kargo", 25m);

        await Assert.ThrowsAsync<ShippingCompanyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonPositiveFee_ThrowsInvalidShippingFeeException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var company = ShippingCompany.Create("Aras Kargo", 25m);
        dbContext.ShippingCompanies.Add(company);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateShippingCompanyCommandHandler(dbContext);
        var command = new UpdateShippingCompanyCommand(company.Id, "Aras Kargo", -1m);

        await Assert.ThrowsAsync<InvalidShippingFeeException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
