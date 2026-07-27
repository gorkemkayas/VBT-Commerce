using FluentAssertions;
using Order.Application.Queries.Guest.GetGuestOrderById;
using Order.Application.Tests.TestSupport;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.Application.Tests.Queries.Guest.GetGuestOrderById;

public class GetGuestOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithOwnedGuestOrder_ReturnsOrderDto()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var guestCustomerId = Guid.NewGuid();
        var order = OrderTestFactory.CreatePendingOrder(guestCustomerId: guestCustomerId);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetGuestOrderByIdQueryHandler(dbContext);

        var result = await handler.Handle(new GetGuestOrderByIdQuery(guestCustomerId, order.Id), CancellationToken.None);

        result.Id.Should().Be(order.Id);
        result.GuestCustomerId.Should().Be(guestCustomerId);
    }

    [Fact]
    public async Task Handle_WithOrderBelongingToAnotherGuest_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var order = OrderTestFactory.CreatePendingOrder(guestCustomerId: Guid.NewGuid());
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetGuestOrderByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new GetGuestOrderByIdQuery(Guid.NewGuid(), order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ThrowsOrderNotFoundException()
    {
        using var dbContext = TestOrderDbContextFactory.Create();
        var handler = new GetGuestOrderByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.Handle(new GetGuestOrderByIdQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }
}
