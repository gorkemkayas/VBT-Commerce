using Cart.Application.Commands.Anonymous.RemoveAnonymousCartItem;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.RemoveAnonymousCartItem;

public class RemoveAnonymousCartItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingItem_RemovesItem()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var cart = ShoppingCart.Create(null, anonymousId);
        var (item, _) = cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new RemoveAnonymousCartItemCommandHandler(operations);

        await handler.Handle(new RemoveAnonymousCartItemCommand(anonymousId, item.Id), CancellationToken.None);

        var stored = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.AnonymousId == anonymousId);
        stored.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNoCartForAnonymousId_ThrowsCartNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new RemoveAnonymousCartItemCommandHandler(operations);

        await Assert.ThrowsAsync<CartNotFoundException>(
            () => handler.Handle(new RemoveAnonymousCartItemCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentCartItem_ThrowsCartItemNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var cart = ShoppingCart.Create(null, anonymousId);
        cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new RemoveAnonymousCartItemCommandHandler(operations);

        await Assert.ThrowsAsync<CartItemNotFoundException>(
            () => handler.Handle(new RemoveAnonymousCartItemCommand(anonymousId, Guid.NewGuid()), CancellationToken.None));
    }
}
