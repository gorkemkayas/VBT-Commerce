using BuildingBlocks.Application.Security;
using Cart.Contracts;
using Cart.Domain.Enums;
using Customer.Contracts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Order.Application.Commands.Checkout.PlaceMyOrder;
using Order.Application.Integrations;
using Order.Application.Tests.TestSupport;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Payment.Contracts;
using Pricing.Contracts;
using Pricing.Domain.Enums;
using Shipping.Contracts;
using Xunit;

namespace Order.Application.Tests.Commands.Checkout.PlaceMyOrder;

public class PlaceMyOrderCommandHandlerTests
{
    private static PlaceMyOrderCommand CreateCommand(Guid addressId, Guid? billingAddressId, Guid shippingCompanyId) =>
        new(
            addressId,
            billingAddressId,
            shippingCompanyId,
            [],
            "Jane Doe",
            "4111111111111111",
            "12",
            "2030",
            "123",
            "12345678901");

    private static CartSummaryDto CreateCart() =>
        new(Guid.NewGuid(), [new CartItemSummaryDto(Guid.NewGuid(), CartItemType.Product, 2)]);

    private static PriceCalculationResultDto CreatePriceResult() =>
        new(
            [new PriceCalculationLineDto(Guid.NewGuid(), PriceItemType.Product, 2, 50m, 100m)],
            100m,
            [],
            0m,
            0.1m,
            10m,
            110m);

    private static CustomerAddressSummaryDto CreateAddress(Guid id) =>
        new(id, "Jane Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000", "Bagdat Cd. No:1", null);

    private sealed class Fixture
    {
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<ICustomerIntegrationService> CustomerIntegrationService { get; } = new();
        public Mock<ICartIntegrationService> CartIntegrationService { get; } = new();
        public Mock<IPricingIntegrationService> PricingIntegrationService { get; } = new();
        public Mock<IShippingIntegrationService> ShippingIntegrationService { get; } = new();
        public Mock<IInventoryIntegrationService> InventoryIntegrationService { get; } = new();
        public Mock<IPaymentIntegrationService> PaymentIntegrationService { get; } = new();
        public Guid UserId { get; } = Guid.NewGuid();

        public Fixture()
        {
            CurrentUserService.Setup(x => x.UserId).Returns(UserId);
            CurrentUserService.Setup(x => x.Email).Returns("jane@example.com");
            CurrentUserService.Setup(x => x.IpAddress).Returns("127.0.0.1");
        }

        public PlaceMyOrderCommandHandler CreateHandler(Order.Application.Abstractions.IOrderDbContext dbContext)
        {
            var orderOperations = OrderTestFactory.CreateOrderOperations(
                dbContext, ShippingIntegrationService, InventoryIntegrationService, PricingIntegrationService, PaymentIntegrationService);

            return new PlaceMyOrderCommandHandler(
                CurrentUserService.Object,
                CustomerIntegrationService.Object,
                CartIntegrationService.Object,
                PricingIntegrationService.Object,
                orderOperations);
        }

        public void SetupHappyPath(Guid addressId, Guid customerId, Guid shippingCompanyId, Guid? billingAddressId = null)
        {
            CustomerIntegrationService
                .Setup(x => x.GetCustomerByUserIdAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CustomerSummaryDto(customerId, UserId, "5551234567"));
            CartIntegrationService
                .Setup(x => x.GetCartByUserIdAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateCart());
            CustomerIntegrationService
                .Setup(x => x.GetCustomerAddressAsync(customerId, addressId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateAddress(addressId));
            if (billingAddressId is { } billingId && billingId != addressId)
            {
                CustomerIntegrationService
                    .Setup(x => x.GetCustomerAddressAsync(customerId, billingId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(CreateAddress(billingId));
            }
            PricingIntegrationService
                .Setup(x => x.CalculateForCustomerAsync(UserId, It.IsAny<IReadOnlyCollection<PriceCalculationItem>>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreatePriceResult());
            ShippingIntegrationService
                .Setup(x => x.GetActiveShippingCompanyAsync(shippingCompanyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ShippingCompanySummaryDto(shippingCompanyId, "Fast Cargo", 25m));
            ShippingIntegrationService
                .Setup(x => x.CreateShipmentAsync(It.IsAny<Guid>(), shippingCompanyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
            PaymentIntegrationService
                .Setup(x => x.ChargeAsync(
                    It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<PaymentCardInfo>(), It.IsAny<PaymentBuyerInfo>(),
                    It.IsAny<PaymentAddressInfo>(), It.IsAny<PaymentAddressInfo>(), It.IsAny<IReadOnlyCollection<PaymentBasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
        }
    }

    [Fact]
    public async Task Handle_WithValidDataAndSuccessfulPayment_PlacesAndConfirmsOrder()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var addressId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        fixture.SetupHappyPath(addressId, customerId, shippingCompanyId);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(addressId, null, shippingCompanyId);

        var orderId = await handler.Handle(command, CancellationToken.None);

        orderId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Orders.FindAsync(orderId);
        stored.Should().NotBeNull();
        stored!.Status.Should().Be(OrderStatus.Confirmed);
        stored.UserId.Should().Be(fixture.UserId);
        stored.BillingRecipientName.Should().BeNull();
        fixture.CartIntegrationService.Verify(x => x.ClearByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDistinctBillingAddress_SnapshotsBillingAddress()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var addressId = Guid.NewGuid();
        var billingAddressId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        fixture.SetupHappyPath(addressId, customerId, shippingCompanyId, billingAddressId);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(addressId, billingAddressId, shippingCompanyId);

        var orderId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Orders.FindAsync(orderId);
        stored!.BillingRecipientName.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task Handle_WithSameBillingAddressIdAsShippingAddress_DoesNotSnapshotBillingAddress()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var addressId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        fixture.SetupHappyPath(addressId, customerId, shippingCompanyId, addressId);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(addressId, addressId, shippingCompanyId);

        var orderId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Orders.FindAsync(orderId);
        stored!.BillingRecipientName.Should().BeNull();
        fixture.CustomerIntegrationService.Verify(
            x => x.GetCustomerAddressAsync(customerId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoCustomerProfile_ThrowsOrderCustomerProfileNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerSummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(Guid.NewGuid(), null, Guid.NewGuid());

        await Assert.ThrowsAsync<OrderCustomerProfileNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyCart_ThrowsOrderCartEmptyException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var customerId = Guid.NewGuid();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerSummaryDto(customerId, fixture.UserId, "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CartSummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(Guid.NewGuid(), null, Guid.NewGuid());

        await Assert.ThrowsAsync<OrderCartEmptyException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentAddress_ThrowsOrderAddressNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var customerId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerSummaryDto(customerId, fixture.UserId, "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCart());
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerAddressAsync(customerId, addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerAddressSummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(addressId, null, Guid.NewGuid());

        await Assert.ThrowsAsync<OrderAddressNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentBillingAddress_ThrowsOrderAddressNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var customerId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var billingAddressId = Guid.NewGuid();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerSummaryDto(customerId, fixture.UserId, "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCart());
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerAddressAsync(customerId, addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateAddress(addressId));
        fixture.CustomerIntegrationService
            .Setup(x => x.GetCustomerAddressAsync(customerId, billingAddressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerAddressSummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(addressId, billingAddressId, Guid.NewGuid());

        await Assert.ThrowsAsync<OrderAddressNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInventoryReservationFailure_PropagatesExceptionWithoutPersistingOrder()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        var addressId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        fixture.SetupHappyPath(addressId, customerId, shippingCompanyId);
        fixture.InventoryIntegrationService
            .Setup(x => x.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<ReserveStockLineItem>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OrderInsufficientStockException("not enough stock"));

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(addressId, null, shippingCompanyId);

        await Assert.ThrowsAsync<OrderInsufficientStockException>(
            () => handler.Handle(command, CancellationToken.None));

        (await dbContext.Orders.CountAsync(CancellationToken.None)).Should().Be(0);
    }
}
