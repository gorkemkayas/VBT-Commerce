using BuildingBlocks.Application.Security;
using Customer.Application.Commands.Addresses.RemoveMyCustomerAddress;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class RemoveMyCustomerAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAddress_RemovesAddress()
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

        var handler = new RemoveMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);

        await handler.Handle(new RemoveMyCustomerAddressCommand(address.Id), CancellationToken.None);

        var stored = await dbContext.CustomerAddresses.FindAsync(address.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new RemoveMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(new RemoveMyCustomerAddressCommand(Guid.NewGuid()), CancellationToken.None));
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

        var handler = new RemoveMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);

        await Assert.ThrowsAsync<CustomerAddressNotFoundException>(
            () => handler.Handle(new RemoveMyCustomerAddressCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
