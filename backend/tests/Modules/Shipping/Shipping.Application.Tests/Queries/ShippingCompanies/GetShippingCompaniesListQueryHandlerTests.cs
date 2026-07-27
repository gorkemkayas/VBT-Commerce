using FluentAssertions;
using Shipping.Application.Queries.ShippingCompanies.GetShippingCompaniesList;
using Shipping.Domain.Entities;
using Xunit;

namespace Shipping.Application.Tests.Queries.ShippingCompanies;

public class GetShippingCompaniesListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllCompaniesPaged()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        dbContext.ShippingCompanies.AddRange(
            ShippingCompany.Create("Aras Kargo", 25m),
            ShippingCompany.Create("Yurtiçi Kargo", 20m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShippingCompaniesListQueryHandler(dbContext);

        var result = await handler.Handle(new GetShippingCompaniesListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsRequestedPage()
    {
        using var dbContext = TestShippingDbContextFactory.Create();
        for (var i = 0; i < 5; i++)
            dbContext.ShippingCompanies.Add(ShippingCompany.Create($"Company {i}", 10m + i));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetShippingCompaniesListQueryHandler(dbContext);

        var result = await handler.Handle(
            new GetShippingCompaniesListQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
}
