using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using Customer.Application.Queries.GuestCustomers.GetGuestCustomerById;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Queries.GuestCustomers;

public class GetGuestCustomerByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingGuestCustomer_ReturnsDto()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var guestCustomer = GuestCustomer.Create("Jane", "Doe", "jane@example.com", "5551234567");
        dbContext.GuestCustomers.Add(guestCustomer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetGuestCustomerByIdQueryHandler(dbContext);

        var dto = await handler.Handle(new GetGuestCustomerByIdQuery(guestCustomer.Id), CancellationToken.None);

        dto.Id.Should().Be(guestCustomer.Id);
        dto.FirstName.Should().Be("Jane");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task Handle_WithNonExistentGuestCustomer_ThrowsGuestCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var handler = new GetGuestCustomerByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<GuestCustomerNotFoundException>(
            () => handler.Handle(new GetGuestCustomerByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
