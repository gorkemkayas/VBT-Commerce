using FluentAssertions;
using Pricing.Application.Commands.Prices.UpdatePrice;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Commands.Prices.UpdatePrice;

public class UpdatePriceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingPrice_UpdatesAmount()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var price = Price.Create(Guid.NewGuid(), PriceItemType.Product, 10m);
        dbContext.Prices.Add(price);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdatePriceCommandHandler(dbContext);

        await handler.Handle(new UpdatePriceCommand(price.Id, 25m), CancellationToken.None);

        var stored = await dbContext.Prices.FindAsync(price.Id);
        stored!.Amount.Should().Be(25m);
        stored.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentPrice_ThrowsPriceNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new UpdatePriceCommandHandler(dbContext);

        await Assert.ThrowsAsync<PriceNotFoundException>(
            () => handler.Handle(new UpdatePriceCommand(Guid.NewGuid(), 25m), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonPositiveAmount_ThrowsInvalidPriceAmountException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var price = Price.Create(Guid.NewGuid(), PriceItemType.Product, 10m);
        dbContext.Prices.Add(price);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdatePriceCommandHandler(dbContext);

        await Assert.ThrowsAsync<InvalidPriceAmountException>(
            () => handler.Handle(new UpdatePriceCommand(price.Id, 0m), CancellationToken.None));
    }
}
