using BuildingBlocks.Application.Security;
using Customer.Application.Commands.Addresses.SetDefaultMyCustomerAddress;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class SetDefaultMyCustomerAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAddress_SetsAsDefault()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), null, null);
        var first = customer.AddAddress(
            "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, true, true, true);
        var second = customer.AddAddress(
            "Work", "John Doe", "5551234567", "Turkey", "Istanbul", "Besiktas", "34100",
            "Some street 2", null, false, true, true);
        dbContext.Customers.Add(customer);
        dbContext.CustomerAddresses.AddRange(first, second);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(customer.UserId);

        var handler = new SetDefaultMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);

        await handler.Handle(new SetDefaultMyCustomerAddressCommand(second.Id), CancellationToken.None);

        (await dbContext.CustomerAddresses.FindAsync(second.Id))!.IsDefault.Should().BeTrue();
        (await dbContext.CustomerAddresses.FindAsync(first.Id))!.IsDefault.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new SetDefaultMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(new SetDefaultMyCustomerAddressCommand(Guid.NewGuid()), CancellationToken.None));
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

        var handler = new SetDefaultMyCustomerAddressCommandHandler(dbContext, currentUserService.Object);

        await Assert.ThrowsAsync<CustomerAddressNotFoundException>(
            () => handler.Handle(new SetDefaultMyCustomerAddressCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
