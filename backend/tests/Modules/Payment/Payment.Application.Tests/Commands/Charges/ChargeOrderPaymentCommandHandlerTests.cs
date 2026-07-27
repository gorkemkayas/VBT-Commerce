using Moq;
using Payment.Application.Commands.Charges.ChargeOrderPayment;
using Payment.Application.Gateway;
using Payment.Application.Services;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Payment.Application.Tests.Commands.Charges;

public class ChargeOrderPaymentCommandHandlerTests
{
    private static ChargeOrderPaymentCommand BuildCommand(Guid? orderId = null) => new(
        orderId ?? Guid.NewGuid(),
        100m,
        100m,
        new IyzicoCardInfo("John Doe", "5528790000000008", "12", "2030", "123"),
        new IyzicoBuyerInfo("John", "Doe", "john@example.com", "12345678901", "5551234567", "127.0.0.1"),
        new IyzicoAddressInfo("Main St", "Istanbul", "Turkey", "34000"),
        new IyzicoAddressInfo("Main St", "Istanbul", "Turkey", "34000"),
        [new IyzicoBasketItem("Item", "Category", 100m)]);

    [Fact]
    public async Task Handle_WithSuccessfulGatewayCharge_PersistsPaymentAndReturnsId()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var gatewayMock = new Mock<IIyzicoGateway>();
        gatewayMock
            .Setup(g => g.ChargeAsync(It.IsAny<IyzicoChargeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IyzicoChargeResult(true, "prov-123", "VISA", "Bonus", "1234", null));

        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new ChargeOrderPaymentCommandHandler(operations);
        var command = BuildCommand();

        var paymentId = await handler.Handle(command, CancellationToken.None);

        paymentId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Payments.FindAsync(paymentId);
        stored.Should().NotBeNull();
        stored!.OrderId.Should().Be(command.OrderId);
        stored.ProviderPaymentId.Should().Be("prov-123");
        stored.Amount.Should().Be(command.PaidTotal);
        stored.CardAssociation.Should().Be("VISA");
        stored.CardFamily.Should().Be("Bonus");
        stored.CardLastFourDigits.Should().Be("1234");
        stored.Status.Should().Be(PaymentStatus.Succeeded);
        gatewayMock.Verify(
            g => g.ChargeAsync(
                It.Is<IyzicoChargeRequest>(r => r.OrderId == command.OrderId && r.PaidTotal == command.PaidTotal),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDeclinedGatewayCharge_ThrowsPaymentDeclinedExceptionAndDoesNotPersist()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var gatewayMock = new Mock<IIyzicoGateway>();
        gatewayMock
            .Setup(g => g.ChargeAsync(It.IsAny<IyzicoChargeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IyzicoChargeResult(false, null, null, null, null, "Insufficient funds"));

        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new ChargeOrderPaymentCommandHandler(operations);
        var command = BuildCommand();

        await Assert.ThrowsAsync<PaymentDeclinedException>(
            () => handler.Handle(command, CancellationToken.None));

        dbContext.Payments.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CalledTwiceForSameOrder_CreatesTwoIndependentPaymentRecords()
    {
        // The domain has no "already charged" guard: a successful charge always persists a new
        // PaymentTransaction row, and OrderId uniqueness is not enforced at this layer.
        using var dbContext = TestPaymentDbContextFactory.Create();
        var gatewayMock = new Mock<IIyzicoGateway>();
        gatewayMock
            .Setup(g => g.ChargeAsync(It.IsAny<IyzicoChargeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IyzicoChargeResult(true, "prov-123", "VISA", "Bonus", "1234", null));

        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new ChargeOrderPaymentCommandHandler(operations);
        var orderId = Guid.NewGuid();

        var firstId = await handler.Handle(BuildCommand(orderId), CancellationToken.None);
        var secondId = await handler.Handle(BuildCommand(orderId), CancellationToken.None);

        firstId.Should().NotBe(secondId);
        dbContext.Payments.Count(p => p.OrderId == orderId).Should().Be(2);
    }
}
