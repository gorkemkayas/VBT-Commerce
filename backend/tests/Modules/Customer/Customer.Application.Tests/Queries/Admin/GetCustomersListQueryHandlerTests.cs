using Customer.Domain.Entities;
using Customer.Application.Queries.Admin.GetCustomersList;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Queries.Admin;

public class GetCustomersListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithMultipleCustomers_ReturnsPagedResult()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        for (var i = 0; i < 5; i++)
            dbContext.Customers.Add(CustomerProfile.Create(Guid.NewGuid(), null, null));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCustomersListQueryHandler(dbContext);

        var result = await handler.Handle(new GetCustomersListQuery(1, 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithNoCustomers_ReturnsEmptyResult()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var handler = new GetCustomersListQueryHandler(dbContext);

        var result = await handler.Handle(new GetCustomersListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }
}
