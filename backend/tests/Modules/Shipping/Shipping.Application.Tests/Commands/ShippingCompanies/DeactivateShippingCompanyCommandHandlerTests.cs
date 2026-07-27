using FluentAssertions;
using Shipping.Application.Commands.ShippingCompanies.DeactivateShippingCompany;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Commands.ShippingCompanies;

public class DeactivateShippingCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingActiveCompany_DeactivatesCompany()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var company = ShippingCompany.Create("Aras Kargo", 25m);
        dbContext.ShippingCompanies.Add(company);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateShippingCompanyCommandHandler(dbContext);

        await handler.Handle(new DeactivateShippingCompanyCommand(company.Id), CancellationToken.None);

        var stored = await dbContext.ShippingCompanies.FindAsync(company.Id);
        stored!.IsActive.Should().BeFalse();
        stored.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentCompany_ThrowsShippingCompanyNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new DeactivateShippingCompanyCommandHandler(dbContext);

        await Assert.ThrowsAsync<ShippingCompanyNotFoundException>(
            () => handler.Handle(new DeactivateShippingCompanyCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
