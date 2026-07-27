using FluentAssertions;
using Moq;
using Pricing.Application.Integrations;
using Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;
using Pricing.Application.Services;
using Pricing.Contracts;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;
using DomainTaxRate = Pricing.Domain.Entities.TaxRate;

namespace Pricing.Application.Tests.Queries.Calculate.CalculateGuestOrderPrice;

public class CalculateGuestOrderPriceQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingGuestCustomerAndNoCoupons_ComputesTotals()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(10m));
        var itemId = Guid.NewGuid();
        dbContext.Prices.Add(Price.Create(itemId, PriceItemType.Product, 50m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        var customerIntegrationServiceMock = new Mock<ICustomerIntegrationService>();
        customerIntegrationServiceMock
            .Setup(s => s.GuestCustomerExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new PriceCalculationService(dbContext, catalogIntegrationServiceMock.Object, customerIntegrationServiceMock.Object);
        var handler = new CalculateGuestOrderPriceQueryHandler(service);

        var query = new CalculateGuestOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 2)],
            []);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Subtotal.Should().Be(100m);
        result.TaxAmount.Should().Be(10m);
        result.GrandTotal.Should().Be(110m);
    }

    [Fact]
    public async Task Handle_WithNonExistentGuestCustomer_ThrowsPricingGuestCustomerNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(0m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        var customerIntegrationServiceMock = new Mock<ICustomerIntegrationService>();
        customerIntegrationServiceMock
            .Setup(s => s.GuestCustomerExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new PriceCalculationService(dbContext, catalogIntegrationServiceMock.Object, customerIntegrationServiceMock.Object);
        var handler = new CalculateGuestOrderPriceQueryHandler(service);

        var query = new CalculateGuestOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(Guid.NewGuid(), PriceItemType.Product, 1)],
            []);

        await Assert.ThrowsAsync<PricingGuestCustomerNotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithGuestPerUserUsageLimitAlreadyReached_ThrowsCouponUsageLimitExceededException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(0m));
        var guestCustomerId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        dbContext.Prices.Add(Price.Create(itemId, PriceItemType.Product, 100m));
        var coupon = Coupon.Create(
            "GUEST1", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30), null, 1);
        dbContext.Coupons.Add(coupon);
        dbContext.CouponUsages.Add(CouponUsage.Create(coupon.Id, null, guestCustomerId, Guid.NewGuid(), 10m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        var customerIntegrationServiceMock = new Mock<ICustomerIntegrationService>();
        customerIntegrationServiceMock
            .Setup(s => s.GuestCustomerExistsAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new PriceCalculationService(dbContext, catalogIntegrationServiceMock.Object, customerIntegrationServiceMock.Object);
        var handler = new CalculateGuestOrderPriceQueryHandler(service);

        var query = new CalculateGuestOrderPriceQuery(
            guestCustomerId,
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["GUEST1"]);

        await Assert.ThrowsAsync<CouponUsageLimitExceededException>(
            () => handler.Handle(query, CancellationToken.None));
    }
}
