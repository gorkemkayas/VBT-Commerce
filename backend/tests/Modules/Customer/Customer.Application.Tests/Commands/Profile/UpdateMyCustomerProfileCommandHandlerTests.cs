using BuildingBlocks.Application.Security;
using Customer.Application.Commands.Profile.UpdateMyCustomerProfile;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Commands.Profile;

public class UpdateMyCustomerProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProfile_UpdatesProfile()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), "5551234567", null);
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(customer.UserId);

        var handler = new UpdateMyCustomerProfileCommandHandler(dbContext, currentUserService.Object);
        var command = new UpdateMyCustomerProfileCommand("5559876543", new DateOnly(1985, 5, 5));

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Customers.FindAsync(customer.Id);
        stored!.PhoneNumber.Should().Be("5559876543");
        stored.DateOfBirth.Should().Be(new DateOnly(1985, 5, 5));
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new UpdateMyCustomerProfileCommandHandler(dbContext, currentUserService.Object);
        var command = new UpdateMyCustomerProfileCommand(null, null);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
