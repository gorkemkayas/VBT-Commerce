/// Backend'in `CartItemDto` şekline karşılık gelir — `GET /api/carts/...`
/// yanıtındaki sepet kalemlerini temsil eder. Ürün adı/görsel/fiyat
/// içermez; bunlar [CartItemSnapshot] ile yerelde tamamlanır.
class RemoteCartItem {
  const RemoteCartItem({
    required this.id,
    required this.sellableItemId,
    required this.sellableItemType,
    required this.quantity,
  });

  final String id;
  final String sellableItemId;
  final String sellableItemType;
  final int quantity;

  factory RemoteCartItem.fromJson(Map<String, dynamic> json) => RemoteCartItem(
    id: json['id'] as String,
    sellableItemId: json['sellableItemId'] as String,
    sellableItemType: json['sellableItemType'] as String,
    quantity: json['quantity'] as int,
  );
}
