using Cart.Application.Commands.Anonymous.UpdateAnonymousCartItemQuantity;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.UpdateAnonymousCartItemQuantity;

public class UpdateAnonymousCartItemQuantityCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidQuantity_UpdatesItemQuantity()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var cart = ShoppingCart.Create(null, anonymousId);
        var (item, _) = cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext, availableQuantity: 10);
        var handler = new UpdateAnonymousCartItemQuantityCommandHandler(operations);

        await handler.Handle(new UpdateAnonymousCartItemQuantityCommand(anonymousId, item.Id, 5), CancellationToken.None);

        var stored = await dbContext.CartItems.FindAsync(item.Id);
        stored!.Quantity.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WithNoCartForAnonymousId_ThrowsCartNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new UpdateAnonymousCartItemQuantityCommandHandler(operations);

        await Assert.ThrowsAsync<CartNotFoundException>(
            () => handler.Handle(new UpdateAnonymousCartItemQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), 1), CancellationToken.None));
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
        var handler = new UpdateAnonymousCartItemQuantityCommandHandler(operations);

        await Assert.ThrowsAsync<CartItemNotFoundException>(
            () => handler.Handle(new UpdateAnonymousCartItemQuantityCommand(anonymousId, Guid.NewGuid(), 1), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRequestedQuantityExceedsAvailableStock_ThrowsCartInsufficientStockException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var cart = ShoppingCart.Create(null, anonymousId);
        var (item, _) = cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext, availableQuantity: 0);
        var handler = new UpdateAnonymousCartItemQuantityCommandHandler(operations);

        await Assert.ThrowsAsync<CartInsufficientStockException>(
            () => handler.Handle(new UpdateAnonymousCartItemQuantityCommand(anonymousId, item.Id, 1), CancellationToken.None));
    }
}
