using BuildingBlocks.Application.Messaging;
using Inventory.Domain.Enums;

namespace Inventory.Application.Queries.Reservations.GetAvailableQuantity;

public record GetAvailableQuantityQuery(Guid SellableItemId, InventoryItemType SellableItemType) : IQuery<int>;
