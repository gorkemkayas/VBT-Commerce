using FluentAssertions;
using Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Commands.ShippingCompanies;

public class CreateShippingCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithUniqueName_CreatesActiveShippingCompany()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new CreateShippingCompanyCommandHandler(dbContext);
        var command = new CreateShippingCompanyCommand("Aras Kargo", 25m);

        var companyId = await handler.Handle(command, CancellationToken.None);

        companyId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.ShippingCompanies.FindAsync(companyId);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Aras Kargo");
        stored.Fee.Should().Be(25m);
        stored.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ThrowsShippingCompanyAlreadyExistsException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        dbContext.ShippingCompanies.Add(ShippingCompany.Create("Aras Kargo", 25m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateShippingCompanyCommandHandler(dbContext);
        var command = new CreateShippingCompanyCommand("Aras Kargo", 30m);

        await Assert.ThrowsAsync<ShippingCompanyAlreadyExistsException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonPositiveFee_ThrowsInvalidShippingFeeException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new CreateShippingCompanyCommandHandler(dbContext);
        var command = new CreateShippingCompanyCommand("Aras Kargo", 0m);

        await Assert.ThrowsAsync<InvalidShippingFeeException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
