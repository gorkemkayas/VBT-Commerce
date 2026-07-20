using Order.Domain.Enums;

namespace Order.Application.Integrations;

public record ReserveStockLineItem(Guid SellableItemId, OrderItemType SellableItemType, int Quantity);
