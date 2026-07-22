/// `OrderItemDto`'ya karşılık gelir. Backend bu satırda ürün **adı**
/// döndürmüyor (sadece `sellableItemId`/`sellableItemType`) — ad, sunum
/// katmanında Product feature'ın `productDetailProvider`'ı üzerinden ayrıca
/// çözülür (yalnızca `Product` tipindeki kalemler için; `Variant` id'sinden
/// ürüne geri dönen bir endpoint yok).
class OrderItem {
  const OrderItem({
    required this.sellableItemId,
    required this.sellableItemType,
    required this.quantity,
    required this.unitPrice,
    required this.lineSubtotal,
  });

  final String sellableItemId;

  /// `"Product"` | `"Variant"`.
  final String sellableItemType;
  final int quantity;
  final double unitPrice;
  final double lineSubtotal;
}
