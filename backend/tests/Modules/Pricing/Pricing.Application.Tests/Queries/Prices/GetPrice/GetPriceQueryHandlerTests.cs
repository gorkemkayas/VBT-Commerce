using FluentAssertions;
using Pricing.Application.Queries.Prices.GetPrice;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Queries.Prices.GetPrice;

public class GetPriceQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingPrice_ReturnsPriceDto()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var itemId = Guid.NewGuid();
        var price = Price.Create(itemId, PriceItemType.Product, 42m);
        dbContext.Prices.Add(price);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPriceQueryHandler(dbContext);

        var result = await handler.Handle(new GetPriceQuery(itemId, PriceItemType.Product), CancellationToken.None);

        result.Amount.Should().Be(42m);
        result.SellableItemId.Should().Be(itemId);
    }

    [Fact]
    public async Task Handle_WithNoMatchingPrice_ThrowsPriceNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new GetPriceQueryHandler(dbContext);

        await Assert.ThrowsAsync<PriceNotFoundException>(
            () => handler.Handle(new GetPriceQuery(Guid.NewGuid(), PriceItemType.Product), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithMatchingIdButDifferentType_ThrowsPriceNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var itemId = Guid.NewGuid();
        dbContext.Prices.Add(Price.Create(itemId, PriceItemType.Product, 42m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPriceQueryHandler(dbContext);

        await Assert.ThrowsAsync<PriceNotFoundException>(
            () => handler.Handle(new GetPriceQuery(itemId, PriceItemType.Variant), CancellationToken.None));
    }
}
