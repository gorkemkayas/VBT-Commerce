using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using Customer.Application.Queries.Admin.GetCustomerByUserId;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Queries.Admin;

public class GetCustomerByUserIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCustomer_ReturnsDto()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), "5551234567", new DateOnly(1990, 1, 1));
        var address = customer.AddAddress(
            "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, true, true, true);
        dbContext.Customers.Add(customer);
        dbContext.CustomerAddresses.Add(address);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCustomerByUserIdQueryHandler(dbContext);

        var dto = await handler.Handle(new GetCustomerByUserIdQuery(customer.UserId), CancellationToken.None);

        dto.Id.Should().Be(customer.Id);
        dto.UserId.Should().Be(customer.UserId);
        dto.PhoneNumber.Should().Be("5551234567");
        dto.Addresses.Should().ContainSingle(a => a.Id == address.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var handler = new GetCustomerByUserIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(new GetCustomerByUserIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
