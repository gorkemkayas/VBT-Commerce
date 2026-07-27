using FluentAssertions;
using Payment.Application.Queries.GetPaymentsList;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Xunit;

namespace Payment.Application.Tests.Queries.GetPaymentsList;

public class GetPaymentsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoPayments_ReturnsEmptyPagedResult()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var handler = new GetPaymentsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetPaymentsListQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithMultiplePayments_ReturnsAllMappedToDto()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var first = PaymentTransaction.Create(Guid.NewGuid(), "prov-1", 50m, "VISA", "Bonus", "1111");
        var second = PaymentTransaction.Create(Guid.NewGuid(), "prov-2", 75m, "MASTERCARD", "World", "2222");
        dbContext.Payments.AddRange(first, second);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPaymentsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetPaymentsListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().Contain(p => p.ProviderPaymentId == "prov-1");
        result.Items.Should().Contain(p => p.ProviderPaymentId == "prov-2");
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsOnlyMatchingPayments()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        var succeeded = PaymentTransaction.Create(Guid.NewGuid(), "prov-1", 50m, "VISA", "Bonus", "1111");
        var refunded = PaymentTransaction.Create(Guid.NewGuid(), "prov-2", 75m, "MASTERCARD", "World", "2222");
        refunded.MarkRefunded();
        dbContext.Payments.AddRange(succeeded, refunded);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPaymentsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetPaymentsListQuery(PaymentStatus.Refunded), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Single().ProviderPaymentId.Should().Be("prov-2");
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsRequestedPageOnly()
    {
        using var dbContext = TestPaymentDbContextFactory.Create();
        for (var i = 0; i < 5; i++)
        {
            dbContext.Payments.Add(PaymentTransaction.Create(Guid.NewGuid(), $"prov-{i}", 10m, null, null, null));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPaymentsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetPaymentsListQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
}
