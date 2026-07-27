using BuildingBlocks.Application.Security;
using FluentAssertions;
using Moq;
using Pricing.Application.Integrations;
using Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;
using Pricing.Application.Services;
using Pricing.Contracts;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;
using DomainTaxRate = Pricing.Domain.Entities.TaxRate;

namespace Pricing.Application.Tests.Queries.Calculate.CalculateMyOrderPrice;

public class CalculateMyOrderPriceQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithCurrentUser_ComputesTotalsForThatCustomer()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(0m));
        var itemId = Guid.NewGuid();
        dbContext.Prices.Add(Price.Create(itemId, PriceItemType.Product, 40m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        var customerIntegrationServiceMock = new Mock<ICustomerIntegrationService>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(s => s.UserId).Returns(Guid.NewGuid());

        var service = new PriceCalculationService(dbContext, catalogIntegrationServiceMock.Object, customerIntegrationServiceMock.Object);
        var handler = new CalculateMyOrderPriceQueryHandler(service, currentUserServiceMock.Object);

        var query = new CalculateMyOrderPriceQuery(
            [new PriceCalculationItem(itemId, PriceItemType.Product, 3)],
            []);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Subtotal.Should().Be(120m);
        result.GrandTotal.Should().Be(120m);
    }

    [Fact]
    public async Task Handle_WithPerUserUsageLimitAlreadyReachedByCurrentUser_ThrowsCouponUsageLimitExceededException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(0m));
        var customerId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        dbContext.Prices.Add(Price.Create(itemId, PriceItemType.Product, 40m));
        var coupon = Coupon.Create(
            "ME1", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30), null, 1);
        dbContext.Coupons.Add(coupon);
        dbContext.CouponUsages.Add(CouponUsage.Create(coupon.Id, customerId, null, Guid.NewGuid(), 4m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        var customerIntegrationServiceMock = new Mock<ICustomerIntegrationService>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(s => s.UserId).Returns(customerId);

        var service = new PriceCalculationService(dbContext, catalogIntegrationServiceMock.Object, customerIntegrationServiceMock.Object);
        var handler = new CalculateMyOrderPriceQueryHandler(service, currentUserServiceMock.Object);

        var query = new CalculateMyOrderPriceQuery(
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["ME1"]);

        await Assert.ThrowsAsync<CouponUsageLimitExceededException>(
            () => handler.Handle(query, CancellationToken.None));
    }
}
