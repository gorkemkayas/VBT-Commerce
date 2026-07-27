using BuildingBlocks.Application.Security;
using Customer.Application.Commands.Addresses.AddMyCustomerAddress;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class AddMyCustomerAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCustomer_AddsAddress()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), null, null);
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(customer.UserId);

        var handler = new AddMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);
        var command = new AddMyCustomerAddressCommand(
            "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, false, true, true);

        var addressId = await handler.Handle(command, CancellationToken.None);

        addressId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.CustomerAddresses.FindAsync(addressId);
        stored.Should().NotBeNull();
        stored!.Label.Should().Be("Home");
        stored.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new AddMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);
        var command = new AddMyCustomerAddressCommand(
            "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, false, true, true);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
