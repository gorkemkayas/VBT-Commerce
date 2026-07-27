using Cart.Application.Commands.MergeAnonymousCart;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.MergeAnonymousCart;

public class MergeAnonymousCartCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNoAnonymousCart_DoesNothing()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new MergeAnonymousCartCommandHandler(operations);
        var userId = Guid.NewGuid();
        var anonymousId = Guid.NewGuid();

        await handler.Handle(new MergeAnonymousCartCommand(userId, anonymousId), CancellationToken.None);

        (await dbContext.Carts.AnyAsync(c => c.UserId == userId)).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithEmptyAnonymousCart_RemovesAnonymousCartWithoutCreatingUserCart()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var anonymousCart = ShoppingCart.Create(null, anonymousId);
        dbContext.Carts.Add(anonymousCart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new MergeAnonymousCartCommandHandler(operations);
        var userId = Guid.NewGuid();

        await handler.Handle(new MergeAnonymousCartCommand(userId, anonymousId), CancellationToken.None);

        (await dbContext.Carts.AnyAsync(c => c.AnonymousId == anonymousId)).Should().BeFalse();
        (await dbContext.Carts.AnyAsync(c => c.UserId == userId)).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithItemsAndNoExistingUserCart_CreatesUserCartWithMovedItems()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var sellableItemId = Guid.NewGuid();
        var anonymousCart = ShoppingCart.Create(null, anonymousId);
        anonymousCart.AddItem(sellableItemId, CartItemType.Product, 2);
        dbContext.Carts.Add(anonymousCart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new MergeAnonymousCartCommandHandler(operations);
        var userId = Guid.NewGuid();

        await handler.Handle(new MergeAnonymousCartCommand(userId, anonymousId), CancellationToken.None);

        (await dbContext.Carts.AnyAsync(c => c.AnonymousId == anonymousId)).Should().BeFalse();
        var userCart = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.UserId == userId);
        userCart.Items.Should().ContainSingle(i => i.SellableItemId == sellableItemId && i.Quantity == 2);
    }

    [Fact]
    public async Task Handle_WithItemsAndExistingUserCartWithSameSellableItem_MergesQuantities()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var anonymousId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sellableItemId = Guid.NewGuid();

        var anonymousCart = ShoppingCart.Create(null, anonymousId);
        anonymousCart.AddItem(sellableItemId, CartItemType.Product, 2);
        var userCart = ShoppingCart.Create(userId, null);
        userCart.AddItem(sellableItemId, CartItemType.Product, 3);
        dbContext.Carts.AddRange(anonymousCart, userCart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new MergeAnonymousCartCommandHandler(operations);

        await handler.Handle(new MergeAnonymousCartCommand(userId, anonymousId), CancellationToken.None);

        (await dbContext.Carts.AnyAsync(c => c.AnonymousId == anonymousId)).Should().BeFalse();
        var stored = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.UserId == userId);
        stored.Items.Should().ContainSingle(i => i.SellableItemId == sellableItemId && i.Quantity == 5);
    }
}
