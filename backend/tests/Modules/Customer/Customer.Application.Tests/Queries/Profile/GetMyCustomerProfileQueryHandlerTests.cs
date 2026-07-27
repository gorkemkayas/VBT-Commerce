using BuildingBlocks.Application.Security;
using Customer.Domain.Entities;
using Customer.Domain.Exceptions;
using Customer.Application.Queries.Profile.GetMyCustomerProfile;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customer.Application.Tests.Queries.Profile;

public class GetMyCustomerProfileQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProfile_ReturnsDto()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();
        var customer = CustomerProfile.Create(Guid.NewGuid(), "5551234567", new DateOnly(1990, 1, 1));
        var address = customer.AddAddress(
            "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, true, true, true);
        dbContext.Customers.Add(customer);
        dbContext.CustomerAddresses.Add(address);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(customer.UserId);

        var handler = new GetMyCustomerProfileQueryHandler(dbContext, currentUserService.Object);

        var dto = await handler.Handle(new GetMyCustomerProfileQuery(), CancellationToken.None);

        dto.Id.Should().Be(customer.Id);
        dto.PhoneNumber.Should().Be("5551234567");
        dto.Addresses.Should().ContainSingle(a => a.Id == address.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentProfile_ThrowsCustomerNotFoundException()
    {
        using var dbContext = TestCustomerDbContextFactory.Create();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new GetMyCustomerProfileQueryHandler(dbContext, currentUserService.Object);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.Handle(new GetMyCustomerProfileQuery(), CancellationToken.None));
    }
}
