using Cart.Application.Queries.Me.GetMyCart;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Queries.Me.GetMyCart;

public class GetMyCartQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCart_ReturnsCartDtoWithItems()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var sellableItemId = Guid.NewGuid();
        var cart = ShoppingCart.Create(userId, null);
        cart.AddItem(sellableItemId, CartItemType.Product, 2);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new GetMyCartQueryHandler(operations, currentUserService);

        var dto = await handler.Handle(new GetMyCartQuery(), CancellationToken.None);

        dto.UserId.Should().Be(userId);
        dto.AnonymousId.Should().BeNull();
        dto.Items.Should().ContainSingle(i => i.SellableItemId == sellableItemId && i.Quantity == 2);
    }

    [Fact]
    public async Task Handle_WithNoCartForUser_ReturnsEmptyTransientCartDto()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var userId = Guid.NewGuid();
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new GetMyCartQueryHandler(operations, currentUserService);

        var dto = await handler.Handle(new GetMyCartQuery(), CancellationToken.None);

        dto.UserId.Should().Be(userId);
        dto.Items.Should().BeEmpty();
    }
}
