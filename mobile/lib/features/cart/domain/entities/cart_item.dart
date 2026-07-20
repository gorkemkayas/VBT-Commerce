class CartItem {
  const CartItem({
    required this.id,
    required this.sellableItemId,
    required this.sellableItemType,
    required this.quantity,
    required this.title,
    required this.imageUrl,
    required this.unitPrice,
  });

  /// Backend'in sepet kalemi kimliği — miktar güncelleme/silme bu id ile yapılır.
  final String id;

  /// Sepete eklenen ürün varyantının kimliği (`Product.variantId`).
  final String sellableItemId;

  /// Backend'in beklediği sabit değer: `"Variant"`.
  final String sellableItemType;
  final int quantity;

  /// Ürün adı, görsel ve fiyat backend'in sepet yanıtında yer almadığı için
  /// "Sepete Ekle" anında yerelde saklanan bir anlık görüntüdür (snapshot).
  final String title;
  final String imageUrl;
  final double unitPrice;

  double get lineTotal => unitPrice * quantity;
}
