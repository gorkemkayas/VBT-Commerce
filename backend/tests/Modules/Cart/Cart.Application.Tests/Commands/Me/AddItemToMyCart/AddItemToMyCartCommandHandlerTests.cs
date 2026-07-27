using Cart.Application.Commands.Me.AddItemToMyCart;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.Me.AddItemToMyCart;

public class AddItemToMyCartCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNoExistingCart_CreatesCartAndAddsItem()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new AddItemToMyCartCommandHandler(operations, currentUserService);
        var sellableItemId = Guid.NewGuid();
        var command = new AddItemToMyCartCommand(sellableItemId, CartItemType.Product, 2);

        var itemId = await handler.Handle(command, CancellationToken.None);

        itemId.Should().NotBe(Guid.Empty);
        var cart = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.UserId == userId);
        cart.Items.Should().ContainSingle(i => i.Id == itemId && i.Quantity == 2);
    }

    [Fact]
    public async Task Handle_WithExistingItemForSameSellableItem_MergesQuantity()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var sellableItemId = Guid.NewGuid();
        var cart = ShoppingCart.Create(userId, null);
        cart.AddItem(sellableItemId, CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext, availableQuantity: 10);
        var currentUserService = TestCurrentUserServiceFactory.Create(userId);
        var handler = new AddItemToMyCartCommandHandler(operations, currentUserService);
        var command = new AddItemToMyCartCommand(sellableItemId, CartItemType.Product, 3);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.UserId == userId);
        stored.Items.Should().ContainSingle(i => i.SellableItemId == sellableItemId && i.Quantity == 4);
    }

    [Fact]
    public async Task Handle_WhenSellableItemDoesNotExist_ThrowsCartSellableItemNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext, sellableItemExists: false);
        var currentUserService = TestCurrentUserServiceFactory.Create(Guid.NewGuid());
        var handler = new AddItemToMyCartCommandHandler(operations, currentUserService);
        var command = new AddItemToMyCartCommand(Guid.NewGuid(), CartItemType.Product, 1);

        await Assert.ThrowsAsync<CartSellableItemNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRequestedQuantityExceedsAvailableStock_ThrowsCartInsufficientStockException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext, availableQuantity: 0);
        var currentUserService = TestCurrentUserServiceFactory.Create(Guid.NewGuid());
        var handler = new AddItemToMyCartCommandHandler(operations, currentUserService);
        var command = new AddItemToMyCartCommand(Guid.NewGuid(), CartItemType.Product, 1);

        await Assert.ThrowsAsync<CartInsufficientStockException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
