using FluentAssertions;
using Pricing.Application.Queries.TaxRate.GetTaxRate;
using Xunit;
using DomainTaxRate = Pricing.Domain.Entities.TaxRate;

namespace Pricing.Application.Tests.Queries.TaxRate.GetTaxRate;

public class GetTaxRateQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTheSingletonTaxRateValue()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(18.5m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTaxRateQueryHandler(dbContext);

        var result = await handler.Handle(new GetTaxRateQuery(), CancellationToken.None);

        result.Should().Be(18.5m);
    }
}
