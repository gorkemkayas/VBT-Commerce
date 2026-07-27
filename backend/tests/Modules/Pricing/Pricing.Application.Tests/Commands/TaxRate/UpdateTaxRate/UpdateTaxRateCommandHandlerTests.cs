using FluentAssertions;
using Pricing.Application.Commands.TaxRate.UpdateTaxRate;
using Pricing.Domain.Exceptions;
using Xunit;
using DomainTaxRate = Pricing.Domain.Entities.TaxRate;

namespace Pricing.Application.Tests.Commands.TaxRate.UpdateTaxRate;

public class UpdateTaxRateCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRate_UpdatesTheSingletonTaxRate()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(18m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateTaxRateCommandHandler(dbContext);

        await handler.Handle(new UpdateTaxRateCommand(20m), CancellationToken.None);

        var stored = await dbContext.TaxRates.FindAsync(DomainTaxRate.SingletonId);
        stored!.Rate.Should().Be(20m);
    }

    [Fact]
    public async Task Handle_WithRateOutOfRange_ThrowsInvalidTaxRateException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(18m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateTaxRateCommandHandler(dbContext);

        await Assert.ThrowsAsync<InvalidTaxRateException>(
            () => handler.Handle(new UpdateTaxRateCommand(101m), CancellationToken.None));
    }
}
