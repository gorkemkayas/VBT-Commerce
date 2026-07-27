using Cart.Application.Commands.ClearCartByUserId;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cart.Application.Tests.Commands.ClearCartByUserId;

public class ClearCartByUserIdCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithItemsInCart_RemovesAllItems()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var cart = ShoppingCart.Create(userId, null);
        cart.AddItem(Guid.NewGuid(), CartItemType.Product, 1);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new ClearCartByUserIdCommandHandler(operations);

        await handler.Handle(new ClearCartByUserIdCommand(userId), CancellationToken.None);

        var stored = await dbContext.Carts.Include(c => c.Items).FirstAsync(c => c.UserId == userId);
        stored.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNoCartForUserId_ThrowsCartNotFoundException()
    {
        using var dbContext = TestCartDbContextFactory.Create();
        var operations = TestCartOperationsFactory.Create(dbContext);
        var handler = new ClearCartByUserIdCommandHandler(operations);

        await Assert.ThrowsAsync<CartNotFoundException>(
            () => handler.Handle(new ClearCartByUserIdCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
