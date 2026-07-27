using Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Commands.GuestCustomers;

public class CreateGuestCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesGuestCustomer()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var handler = new CreateGuestCustomerCommandHandler(dbContext);
        var command = new CreateGuestCustomerCommand("Jane", "Doe", "JANE@Example.com", "5551234567");

        var guestCustomerId = await handler.Handle(command, CancellationToken.None);

        guestCustomerId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.GuestCustomers.FindAsync(guestCustomerId);
        stored.Should().NotBeNull();
        stored!.FirstName.Should().Be("Jane");
        stored.LastName.Should().Be("Doe");
        stored.Email.Should().Be("jane@example.com");
        stored.PhoneNumber.Should().Be("5551234567");
    }
}
