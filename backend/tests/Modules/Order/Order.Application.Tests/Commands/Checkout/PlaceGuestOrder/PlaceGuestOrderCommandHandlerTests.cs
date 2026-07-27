using BuildingBlocks.Application.Security;
using Cart.Contracts;
using Cart.Domain.Enums;
using Customer.Contracts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Order.Application.Commands.Checkout.PlaceGuestOrder;
using Order.Application.Integrations;
using Order.Application.Tests.TestSupport;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Payment.Contracts;
using Pricing.Contracts;
using Pricing.Domain.Enums;
using Shipping.Contracts;
using Xunit;

namespace Order.Application.Tests.Commands.Checkout.PlaceGuestOrder;

public class PlaceGuestOrderCommandHandlerTests
{
    private static PlaceGuestOrderCommand CreateCommand(Guid guestCustomerId, Guid anonymousId, Guid shippingCompanyId) =>
        new(
            guestCustomerId,
            anonymousId,
            shippingCompanyId,
            [],
            "Jane Doe",
            "5551234567",
            "Turkey",
            "Istanbul",
            "Kadikoy",
            "34000",
            "Bagdat Cd. No:1",
            null,
            null, null, null, null, null, null, null, null,
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

    private sealed class Fixture
    {
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<ICustomerIntegrationService> CustomerIntegrationService { get; } = new();
        public Mock<ICartIntegrationService> CartIntegrationService { get; } = new();
        public Mock<IPricingIntegrationService> PricingIntegrationService { get; } = new();
        public Mock<IShippingIntegrationService> ShippingIntegrationService { get; } = new();
        public Mock<IInventoryIntegrationService> InventoryIntegrationService { get; } = new();
        public Mock<IPaymentIntegrationService> PaymentIntegrationService { get; } = new();

        public Fixture()
        {
            CurrentUserService.Setup(x => x.IpAddress).Returns("127.0.0.1");
        }

        public PlaceGuestOrderCommandHandler CreateHandler(Order.Application.Abstractions.IOrderDbContext dbContext)
        {
            var orderOperations = OrderTestFactory.CreateOrderOperations(
                dbContext, ShippingIntegrationService, InventoryIntegrationService, PricingIntegrationService, PaymentIntegrationService);

            return new PlaceGuestOrderCommandHandler(
                CurrentUserService.Object,
                CustomerIntegrationService.Object,
                CartIntegrationService.Object,
                PricingIntegrationService.Object,
                orderOperations);
        }
    }

    [Fact]
    public async Task Handle_WithValidCartAndSuccessfulPayment_PlacesAndConfirmsOrder()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var anonymousId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        var cart = CreateCart();

        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetGuestCustomerAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GuestCustomerSummaryDto(guestCustomerId, "Jane", "Doe", "jane@example.com", "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByAnonymousIdAsync(anonymousId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        fixture.PricingIntegrationService
            .Setup(x => x.CalculateForGuestAsync(guestCustomerId, It.IsAny<IReadOnlyCollection<PriceCalculationItem>>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePriceResult());
        fixture.ShippingIntegrationService
            .Setup(x => x.GetActiveShippingCompanyAsync(shippingCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShippingCompanySummaryDto(shippingCompanyId, "Fast Cargo", 25m));
        fixture.ShippingIntegrationService
            .Setup(x => x.CreateShipmentAsync(It.IsAny<Guid>(), shippingCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());
        fixture.PaymentIntegrationService
            .Setup(x => x.ChargeAsync(
                It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<PaymentCardInfo>(), It.IsAny<PaymentBuyerInfo>(),
                It.IsAny<PaymentAddressInfo>(), It.IsAny<PaymentAddressInfo>(), It.IsAny<IReadOnlyCollection<PaymentBasketItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(guestCustomerId, anonymousId, shippingCompanyId);

        var orderId = await handler.Handle(command, CancellationToken.None);

        orderId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Orders.FindAsync(orderId);
        stored.Should().NotBeNull();
        stored!.Status.Should().Be(OrderStatus.Confirmed);
        stored.GuestCustomerId.Should().Be(guestCustomerId);
        fixture.CartIntegrationService.Verify(x => x.ClearByAnonymousIdAsync(anonymousId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentGuestCustomer_ThrowsOrderGuestCustomerNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetGuestCustomerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GuestCustomerSummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<OrderGuestCustomerNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNullCart_ThrowsOrderCartEmptyException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetGuestCustomerAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GuestCustomerSummaryDto(guestCustomerId, "Jane", "Doe", "jane@example.com", "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByAnonymousIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CartSummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(guestCustomerId, Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<OrderCartEmptyException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyCartItems_ThrowsOrderCartEmptyException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetGuestCustomerAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GuestCustomerSummaryDto(guestCustomerId, "Jane", "Doe", "jane@example.com", "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByAnonymousIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CartSummaryDto(Guid.NewGuid(), []));

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(guestCustomerId, Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<OrderCartEmptyException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithUnavailableShippingCompany_ThrowsOrderShippingCompanyUnavailableException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetGuestCustomerAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GuestCustomerSummaryDto(guestCustomerId, "Jane", "Doe", "jane@example.com", "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByAnonymousIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCart());
        fixture.PricingIntegrationService
            .Setup(x => x.CalculateForGuestAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<PriceCalculationItem>>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePriceResult());
        fixture.ShippingIntegrationService
            .Setup(x => x.GetActiveShippingCompanyAsync(shippingCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShippingCompanySummaryDto?)null);

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(guestCustomerId, Guid.NewGuid(), shippingCompanyId);

        await Assert.ThrowsAsync<OrderShippingCompanyUnavailableException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithPaymentChargeFailure_ReleasesReservationAndPropagatesException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var shippingCompanyId = Guid.NewGuid();
        var fixture = new Fixture();
        fixture.CustomerIntegrationService
            .Setup(x => x.GetGuestCustomerAsync(guestCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GuestCustomerSummaryDto(guestCustomerId, "Jane", "Doe", "jane@example.com", "5551234567"));
        fixture.CartIntegrationService
            .Setup(x => x.GetCartByAnonymousIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCart());
        fixture.PricingIntegrationService
            .Setup(x => x.CalculateForGuestAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<PriceCalculationItem>>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePriceResult());
        fixture.ShippingIntegrationService
            .Setup(x => x.GetActiveShippingCompanyAsync(shippingCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShippingCompanySummaryDto(shippingCompanyId, "Fast Cargo", 25m));
        fixture.ShippingIntegrationService
            .Setup(x => x.CreateShipmentAsync(It.IsAny<Guid>(), shippingCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());
        fixture.PaymentIntegrationService
            .Setup(x => x.ChargeAsync(
                It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<PaymentCardInfo>(), It.IsAny<PaymentBuyerInfo>(),
                It.IsAny<PaymentAddressInfo>(), It.IsAny<PaymentAddressInfo>(), It.IsAny<IReadOnlyCollection<PaymentBasketItem>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("card declined"));

        var handler = fixture.CreateHandler(dbContext);
        var command = CreateCommand(guestCustomerId, Guid.NewGuid(), shippingCompanyId);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        fixture.InventoryIntegrationService.Verify(
            x => x.ReleaseReservationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        (await dbContext.Orders.CountAsync(CancellationToken.None)).Should().Be(0);
    }
}
