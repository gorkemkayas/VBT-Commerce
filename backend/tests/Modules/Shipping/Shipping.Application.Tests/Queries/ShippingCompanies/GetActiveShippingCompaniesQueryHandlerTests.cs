using FluentAssertions;
using Shipping.Application.Queries.ShippingCompanies.GetActiveShippingCompanies;
using Shipping.Domain.Entities;
using Xunit;

namespace Shipping.Application.Tests.Queries.ShippingCompanies;

public class GetActiveShippingCompaniesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyActiveCompaniesOrderedByName()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var yurtici = ShippingCompany.Create("Yurtiçi Kargo", 20m);
        var aras = ShippingCompany.Create("Aras Kargo", 25m);
        var inactive = ShippingCompany.Create("Inactive Co", 10m);
        inactive.Deactivate();
        dbContext.ShippingCompanies.AddRange(yurtici, aras, inactive);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetActiveShippingCompaniesQueryHandler(dbContext);

        var result = await handler.Handle(new GetActiveShippingCompaniesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(c => c.Name).Should().ContainInOrder("Aras Kargo", "Yurtiçi Kargo");
    }

    [Fact]
    public async Task Handle_WithNoActiveCompanies_ReturnsEmptyList()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        var handler = new GetActiveShippingCompaniesQueryHandler(dbContext);

        var result = await handler.Handle(new GetActiveShippingCompaniesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
