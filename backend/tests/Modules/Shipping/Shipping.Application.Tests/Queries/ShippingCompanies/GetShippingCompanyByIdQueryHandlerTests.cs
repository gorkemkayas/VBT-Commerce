using FluentAssertions;
using Shipping.Application.Queries.ShippingCompanies.GetShippingCompanyById;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;
using Xunit;

namespace Shipping.Application.Tests.Queries.ShippingCompanies;

public class GetShippingCompanyByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCompany_ReturnsShippingCompanyDto()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var company = ShippingCompany.Create("Aras Kargo", 25m);
        dbContext.ShippingCompanies.Add(company);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShippingCompanyByIdQueryHandler(dbContext);

        var result = await handler.Handle(new GetShippingCompanyByIdQuery(company.Id), CancellationToken.None);

        result.Id.Should().Be(company.Id);
        result.Name.Should().Be("Aras Kargo");
        result.Fee.Should().Be(25m);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentCompany_ThrowsShippingCompanyNotFoundException()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new GetShippingCompanyByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<ShippingCompanyNotFoundException>(
            () => handler.Handle(new GetShippingCompanyByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
