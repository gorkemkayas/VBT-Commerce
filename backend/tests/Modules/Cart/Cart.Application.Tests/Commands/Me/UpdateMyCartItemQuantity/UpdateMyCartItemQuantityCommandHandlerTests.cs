using Cart.Application.Commands.Me.UpdateMyCartItemQuantity;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.Me.UpdateMyCartItemQuantity;

public class UpdateMyCartItemQuantityCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidQuantity_UpdatesItemQuantity()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var cart = ShoppingCart.Create(userId, null);
        var (item, _) = cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext, availableQuantity: 10);
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new UpdateMyCartItemQuantityCommandHandler(operations, currentUserService);

        await handler.Handle(new UpdateMyCartItemQuantityCommand(item.Id, 5), CancellationToken.None);

        var stored = await dbContext.CartItems.FindAsync(item.Id);
        stored!.Quantity.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WithNoCartForUser_ThrowsCartNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var currentUserService = TestCurrentUserServiceFactory.Create(Guid.NewGuid());
        var handler = new UpdateMyCartItemQuantityCommandHandler(operations, currentUserService);

        await Assert.ThrowsAsync<CartNotFoundException>(
            () => handler.Handle(new UpdateMyCartItemQuantityCommand(Guid.NewGuid(), 1), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentCartItem_ThrowsCartItemNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var cart = ShoppingCart.Create(userId, null);
        cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new UpdateMyCartItemQuantityCommandHandler(operations, currentUserService);

        await Assert.ThrowsAsync<CartItemNotFoundException>(
            () => handler.Handle(new UpdateMyCartItemQuantityCommand(Guid.NewGuid(), 1), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRequestedQuantityExceedsAvailableStock_ThrowsCartInsufficientStockException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var cart = ShoppingCart.Create(userId, null);
        var (item, _) = cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext, availableQuantity: 0);
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new UpdateMyCartItemQuantityCommandHandler(operations, currentUserService);

        await Assert.ThrowsAsync<CartInsufficientStockException>(
            () => handler.Handle(new UpdateMyCartItemQuantityCommand(item.Id, 1), CancellationToken.None));
    }
}
