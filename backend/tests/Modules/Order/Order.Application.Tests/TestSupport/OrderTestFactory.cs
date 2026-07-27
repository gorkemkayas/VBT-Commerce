using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Order.Application.Abstractions;
using Order.Application.Integrations;
using Order.Application.Services;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Tests.TestSupport;

/// <summary>
/// Shared helpers for building a persisted <see cref="CustomerOrder"/> aggregate and a real
/// <see cref="OrderOperations"/> instance backed by mocked cross-module integration services.
/// </summary>
internal static class OrderTestFactory
{
    public static CustomerOrder CreatePendingOrder(
        Guid? userId = null,
        Guid? guestCustomerId = null,
        Guid? id = null,
        IReadOnlyCollection<(Guid SellableItemId, OrderItemType SellableItemType, int Quantity, decimal UnitPrice)>? items = null,
        IReadOnlyCollection<(string Code, decimal DiscountAmount)>? coupons = null)
    {
        if (userId is null && guestCustomerId is null)
            userId = Guid.NewGuid();

        return CustomerOrder.Create(
            id ?? Guid.NewGuid(),
            userId,
            guestCustomerId,
            "Jane Doe",
            "5551234567",
            "Turkey",
            "Istanbul",
            "Kadikoy",
            "34000",
            "Bagdat Cd. No:1",
            null,
            null, null, null, null, null, null, null, null,
            Guid.NewGuid(),
            Guid.NewGuid(),
            25m,
            Guid.NewGuid(),
            items ?? [(Guid.NewGuid(), OrderItemType.Product, 2, 50m)],
            coupons ?? [],
            100m,
            0m,
            0.1m,
            10m);
    }

    public static OrderOperations CreateOrderOperations(
        IOrderDbContext dbContext,
        Mock<IShippingIntegrationService>? shipping = null,
        Mock<IInventoryIntegrationService>? inventory = null,
        Mock<IPricingIntegrationService>? pricing = null,
        Mock<IPaymentIntegrationService>? payment = null)
    {
        return new OrderOperations(
            dbContext,
            (shipping ?? new Mock<IShippingIntegrationService>()).Object,
            (inventory ?? new Mock<IInventoryIntegrationService>()).Object,
            (pricing ?? new Mock<IPricingIntegrationService>()).Object,
            (payment ?? new Mock<IPaymentIntegrationService>()).Object,
            NullLogger<OrderOperations>.Instance);
    }
}
