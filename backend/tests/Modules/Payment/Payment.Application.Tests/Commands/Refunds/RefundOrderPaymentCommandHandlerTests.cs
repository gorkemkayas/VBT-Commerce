using FluentAssertions;
using Moq;
using Payment.Application.Commands.Refunds.RefundOrderPayment;
using Payment.Application.Gateway;
using Payment.Application.Services;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Xunit;

namespace Payment.Application.Tests.Commands.Refunds;

public class RefundOrderPaymentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithSuccessfulRefund_MarksPaymentRefunded()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var payment = PaymentTransaction.Create(Guid.NewGuid(), "prov-123", 100m, "VISA", "Bonus", "1234");
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var gatewayMock = new Mock<IIyzicoGateway>();
        gatewayMock
            .Setup(g => g.RefundAsync(It.IsAny<IyzicoRefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IyzicoRefundResult(true, null));

        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new RefundOrderPaymentCommandHandler(operations);

        await handler.Handle(new RefundOrderPaymentCommand(payment.OrderId, "127.0.0.1"), CancellationToken.None);

        var stored = await dbContext.Payments.FindAsync(payment.Id);
        stored!.Status.Should().Be(PaymentStatus.Refunded);
        stored.RefundedAt.Should().NotBeNull();
        gatewayMock.Verify(
            g => g.RefundAsync(
                It.Is<IyzicoRefundRequest>(r => r.ProviderPaymentId == "prov-123" && r.Ip == "127.0.0.1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoPaymentForOrder_ThrowsPaymentNotFoundException()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var gatewayMock = new Mock<IIyzicoGateway>();
        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new RefundOrderPaymentCommandHandler(operations);

        await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => handler.Handle(new RefundOrderPaymentCommand(Guid.NewGuid(), "127.0.0.1"), CancellationToken.None));

        gatewayMock.Verify(
            g => g.RefundAsync(It.IsAny<IyzicoRefundRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithAlreadyRefundedPayment_ThrowsPaymentAlreadyRefundedException()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var payment = PaymentTransaction.Create(Guid.NewGuid(), "prov-123", 100m, "VISA", "Bonus", "1234");
        payment.MarkRefunded();
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var gatewayMock = new Mock<IIyzicoGateway>();
        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new RefundOrderPaymentCommandHandler(operations);

        await Assert.ThrowsAsync<PaymentAlreadyRefundedException>(
            () => handler.Handle(new RefundOrderPaymentCommand(payment.OrderId, "127.0.0.1"), CancellationToken.None));

        gatewayMock.Verify(
            g => g.RefundAsync(It.IsAny<IyzicoRefundRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithGatewayRefundFailure_ThrowsPaymentGatewayExceptionAndLeavesPaymentSucceeded()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var payment = PaymentTransaction.Create(Guid.NewGuid(), "prov-123", 100m, "VISA", "Bonus", "1234");
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var gatewayMock = new Mock<IIyzicoGateway>();
        gatewayMock
            .Setup(g => g.RefundAsync(It.IsAny<IyzicoRefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IyzicoRefundResult(false, "Provider timeout"));

        var operations = new PaymentOperations(dbContext, gatewayMock.Object);
        var handler = new RefundOrderPaymentCommandHandler(operations);

        await Assert.ThrowsAsync<PaymentGatewayException>(
            () => handler.Handle(new RefundOrderPaymentCommand(payment.OrderId, "127.0.0.1"), CancellationToken.None));

        var stored = await dbContext.Payments.FindAsync(payment.Id);
        stored!.Status.Should().Be(PaymentStatus.Succeeded);
        stored.RefundedAt.Should().BeNull();
    }
}
