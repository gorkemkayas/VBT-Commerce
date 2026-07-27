using FluentAssertions;
using Payment.Application.Queries.GetPaymentById;
using Payment.Domain.Entities;
using Payment.Domain.Exceptions;
using Xunit;

namespace Payment.Application.Tests.Queries.GetPaymentById;

public class GetPaymentByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingPayment_ReturnsDto()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var payment = PaymentTransaction.Create(Guid.NewGuid(), "prov-123", 150m, "VISA", "Bonus", "1234");
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPaymentByIdQueryHandler(dbContext);

        var dto = await handler.Handle(new GetPaymentByIdQuery(payment.Id), CancellationToken.None);

        dto.Id.Should().Be(payment.Id);
        dto.OrderId.Should().Be(payment.OrderId);
        dto.ProviderPaymentId.Should().Be("prov-123");
        dto.Amount.Should().Be(150m);
        dto.CardAssociation.Should().Be("VISA");
        dto.CardFamily.Should().Be("Bonus");
        dto.CardLastFourDigits.Should().Be("1234");
    }

    [Fact]
    public async Task Handle_WithNonExistentPayment_ThrowsPaymentNotFoundException()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var handler = new GetPaymentByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => handler.Handle(new GetPaymentByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
