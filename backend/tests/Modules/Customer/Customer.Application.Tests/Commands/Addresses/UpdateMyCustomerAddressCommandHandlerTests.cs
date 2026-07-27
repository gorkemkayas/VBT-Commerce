using BuildingBlocks.Application.Security;
using Customer.Application.Commands.Addresses.UpdateMyCustomerAddress;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class UpdateMyCustomerAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAddress_UpdatesAddress()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), null, null);
        var address = customer.AddAddress(
            "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, true, true, true);
        dbContext.Customers.Add(customer);
        dbContext.CustomerAddresses.Add(address);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(customer.UserId);

        var handler = new UpdateMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);
        var command = new UpdateMyCustomerAddressCommand(
            address.Id, "Office", "Jane Doe", "5559876543", "Turkey", "Ankara", "Cankaya", "06000",
            "Other street 2", "Floor 3", true, true, false);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.CustomerAddresses.FindAsync(address.Id);
        stored!.Label.Should().Be("Office");
        stored.City.Should().Be("Ankara");
        stored.IsBillingAddress.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new UpdateMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);
        var command = new UpdateMyCustomerAddressCommand(
            Guid.NewGuid(), "Office", "Jane Doe", "5559876543", "Turkey", "Ankara", "Cankaya", "06000",
            "Other street 2", null, true, true, true);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentAddress_ThrowsCustomerAddressNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), null, null);
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(customer.UserId);

        var handler = new UpdateMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);
        var command = new UpdateMyCustomerAddressCommand(
            Guid.NewGuid(), "Office", "Jane Doe", "5559876543", "Turkey", "Ankara", "Cankaya", "06000",
            "Other street 2", null, true, true, true);

        await Assert.ThrowsAsync<CustomerAddressNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
