using Cart.Application.Commands.Anonymous.ClearAnonymousCart;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.ClearAnonymousCart;

public class ClearAnonymousCartCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithItemsInCart_RemovesAllItems()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var cart = ShoppingCart.Create(null, anonymousId);
        cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        cart.AddItem(Guid.NewGuid(), CartItemType.Variant, 2);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new ClearAnonymousCartCommandHandler(operations);

        await handler.Handle(new ClearAnonymousCartCommand(anonymousId), CancellationToken.None);

        var stored = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.AnonymousId == anonymousId);
        stored.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNoCartForAnonymousId_ThrowsCartNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new ClearAnonymousCartCommandHandler(operations);

        await Assert.ThrowsAsync<CartNotFoundException>(
            () => handler.Handle(new ClearAnonymousCartCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
