using Cart.Application.Abstractions;
using Cart.Application.Integrations;
using Cart.Domain.Entities;
using Cart.Domain.Enums;
using Cart.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Cart.Application.Services;

/// <summary>
/// Business logic shared by the self-service ("me") and anonymous cart command handlers — both sides
/// need identical get-or-create/stock-check/quantity-merge behavior, only the source of the owning
/// identity differs (ICurrentUserService vs. a client-supplied AnonymousId). Not a MediatR handler
/// itself; each command handler resolves its own CartOwnerKey and delegates here.
/// </summary>
public class CartOperations(
    ICartDbContext dbContext,
    ICatalogIntegrationService catalogIntegrationService,
    IInventoryIntegrationService inventoryIntegrationService)
{
    public async Task<ShoppingCart?> GetCartAsync(CartOwnerKey owner, CancellationToken cancellationToken)
        => await FindCartAsync(owner, cancellationToken);

    public async Task<CartItem> AddItemAsync(
        CartOwnerKey owner, Guid sellableItemId, CartItemType sellableItemType, int quantity, CancellationToken cancellationToken)
    {
        var sellableItemExists = await catalogIntegrationService.SellableItemExistsAsync(sellableItemId, sellableItemType, cancellationToken);
        if (!sellableItemExists)
            throw new CartSellableItemNotFoundException(sellableItemId);

        var cart = await FindCartAsync(owner, cancellationToken);
        if (cart is null)
        {
            cart = ShoppingCart.Create(owner.UserId, owner.AnonymousId);
            dbContext.Carts.Add(cart);
        }

        var existingQuantity = cart.Items
            .FirstOrDefault(i => i.SellableItemId == sellableItemId && i.SellableItemType == sellableItemType)
            ?.Quantity ?? 0;

        var availableQuantity = await inventoryIntegrationService.GetAvailableQuantityAsync(sellableItemId, sellableItemType, cancellationToken);
        var requestedTotal = existingQuantity + quantity;
        if (requestedTotal > availableQuantity)
            throw new CartInsufficientStockException(sellableItemId, requestedTotal, availableQuantity);

        var (item, isNew) = cart.AddItem(sellableItemId, sellableItemType, quantity);
        if (isNew)
            dbContext.CartItems.Add(item);

        await dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateItemQuantityAsync(CartOwnerKey owner, Guid cartItemId, int quantity, CancellationToken cancellationToken)
    {
        var cart = await FindCartAsync(owner, cancellationToken) ?? throw NotFoundFor(owner);
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId) ?? throw new CartItemNotFoundException(cartItemId);

        var availableQuantity = await inventoryIntegrationService.GetAvailableQuantityAsync(item.SellableItemId, item.SellableItemType, cancellationToken);
        if (quantity > availableQuantity)
            throw new CartInsufficientStockException(item.SellableItemId, quantity, availableQuantity);

        cart.UpdateItemQuantity(cartItemId, quantity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveItemAsync(CartOwnerKey owner, Guid cartItemId, CancellationToken cancellationToken)
    {
        var cart = await FindCartAsync(owner, cancellationToken) ?? throw NotFoundFor(owner);
        var item = cart.RemoveItem(cartItemId);

        dbContext.CartItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearAsync(CartOwnerKey owner, CancellationToken cancellationToken)
    {
        var cart = await FindCartAsync(owner, cancellationToken) ?? throw NotFoundFor(owner);
        var removedItems = cart.Clear();

        if (removedItems.Count > 0)
            dbContext.CartItems.RemoveRange(removedItems);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Moves every line from the anonymous cart into the user's cart (creating the user's cart if it
    /// doesn't exist yet), combining quantities for matching sellable items, then deletes the
    /// anonymous cart. Used only right after login/register (architecture.md-style orchestration at
    /// the API composition root, not exposed as its own public endpoint).
    /// </summary>
    public async Task MergeAnonymousCartIntoUserCartAsync(Guid userId, Guid anonymousId, CancellationToken cancellationToken)
    {
        var anonymousCart = await FindCartAsync(CartOwnerKey.ForAnonymous(anonymousId), cancellationToken);
        if (anonymousCart is null || anonymousCart.Items.Count == 0)
        {
            if (anonymousCart is not null)
                dbContext.Carts.Remove(anonymousCart);

            return;
        }

        var userCart = await FindCartAsync(CartOwnerKey.ForUser(userId), cancellationToken);
        if (userCart is null)
        {
            userCart = ShoppingCart.Create(userId, null);
            dbContext.Carts.Add(userCart);
        }

        foreach (var sourceItem in anonymousCart.Items.ToList())
        {
            var (item, isNew) = userCart.AddItem(sourceItem.SellableItemId, sourceItem.SellableItemType, sourceItem.Quantity);
            if (isNew)
                dbContext.CartItems.Add(item);
        }

        dbContext.Carts.Remove(anonymousCart);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private Task<ShoppingCart?> FindCartAsync(CartOwnerKey owner, CancellationToken cancellationToken)
    {
        var query = dbContext.Carts.Include(c => c.Items).AsQueryable();

        return owner.UserId is not null
            ? query.FirstOrDefaultAsync(c => c.UserId == owner.UserId, cancellationToken)
            : query.FirstOrDefaultAsync(c => c.AnonymousId == owner.AnonymousId, cancellationToken);
    }

    private static CartNotFoundException NotFoundFor(CartOwnerKey owner)
        => owner.UserId is not null
            ? CartNotFoundException.ForUserId(owner.UserId.Value)
            : CartNotFoundException.ForAnonymousId(owner.AnonymousId!.Value);
}
