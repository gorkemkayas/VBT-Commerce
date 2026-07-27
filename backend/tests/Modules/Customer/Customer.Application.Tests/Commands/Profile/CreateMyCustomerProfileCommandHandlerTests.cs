using BuildingBlocks.Application.Security;
using Customer.Application.Commands.Profile.CreateMyCustomerProfile;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Commands.Profile;

public class CreateMyCustomerProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNoExistingProfile_CreatesProfile()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var userId = Guid.NewGuid();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(userId);

        var handler = new CreateMyCustomerProfileCommandHandler(dbContext, currentUserService.Object);
        var command = new CreateMyCustomerProfileCommand("5551234567", new DateOnly(1990, 1, 1));

        var customerId = await handler.Handle(command, CancellationToken.None);

        customerId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Customers.FindAsync(customerId);
        stored.Should().NotBeNull();
        stored!.UserId.Should().Be(userId);
        stored.PhoneNumber.Should().Be("5551234567");
    }

    [Fact]
    public async Task Handle_WithExistingProfile_ThrowsCustomerProfileAlreadyExistsException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var userId = Guid.NewGuid();
        dbContext.Customers.Add(CustomerProfile.Create(userId, null, null));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(userId);

        var handler = new CreateMyCustomerProfileCommandHandler(dbContext, currentUserService.Object);
        var command = new CreateMyCustomerProfileCommand(null, null);

        await Assert.ThrowsAsync<CustomerProfileAlreadyExistsException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
