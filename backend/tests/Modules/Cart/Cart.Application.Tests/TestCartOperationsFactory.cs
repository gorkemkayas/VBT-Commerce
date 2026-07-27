using Cart.Application.Abstractions;
using Cart.Application.Integrations;
using Cart.Application.Services;
using Cart.Domain.Enums;
using Moq;

namespace Cart.Application.Tests;

/// <summary>
/// CartOperations is a plain concrete class (not exposed behind an interface), so handler tests
/// construct a real instance backed by the in-memory ICartDbContext plus Moq'd integration
/// services (Catalog existence check, Inventory availability check) — the only two collaborators
/// outside the module's own DbContext.
/// </summary>
internal static class TestCartOperationsFactory
{
    public static CartOperations Create(
        ICartDbContext dbContext,
        bool sellableItemExists = true,
        int availableQuantity = int.MaxValue)
    {
        var catalogMock = new Mock<ICatalogIntegrationService>();
        catalogMock
            .Setup(s => s.SellableItemExistsAsync(It.IsAny<Guid>(), It.IsAny<CartItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sellableItemExists);

        var inventoryMock = new Mock<IInventoryIntegrationService>();
        inventoryMock
            .Setup(s => s.GetAvailableQuantityAsync(It.IsAny<Guid>(), It.IsAny<CartItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(availableQuantity);

        return new CartOperations(dbContext, catalogMock.Object, inventoryMock.Object);
    }
}
