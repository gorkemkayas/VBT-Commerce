using Cart.Application.Queries.Anonymous.GetAnonymousCart;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Queries.Anonymous.GetAnonymousCart;

public class GetAnonymousCartQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCart_ReturnsCartDtoWithItems()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var sellableItemId = Guid.NewGuid();
        var cart = ShoppingCart.Create(null, anonymousId);
        cart.AddItem(sellableItemId, CartItemType.Product, 2);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new GetAnonymousCartQueryHandler(operations);

        var dto = await handler.Handle(new GetAnonymousCartQuery(anonymousId), CancellationToken.None);

        dto.AnonymousId.Should().Be(anonymousId);
        dto.UserId.Should().BeNull();
        dto.Items.Should().ContainSingle(i => i.SellableItemId == sellableItemId && i.Quantity == 2);
    }

    [Fact]
    public async Task Handle_WithNoCartForAnonymousId_ReturnsEmptyTransientCartDto()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new GetAnonymousCartQueryHandler(operations);
        var anonymousId = Guid.NewGuid();

        var dto = await handler.Handle(new GetAnonymousCartQuery(anonymousId), CancellationToken.None);

        dto.AnonymousId.Should().Be(anonymousId);
        dto.Items.Should().BeEmpty();
    }
}
