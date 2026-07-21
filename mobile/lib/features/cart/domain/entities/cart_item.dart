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

  /// Sepete eklenen ürünün kimliği (`Product.id`).
  final String sellableItemId;

  /// Backend'in sepet yanıtında döndürdüğü tip adı (ör. `"Product"`).
  final String sellableItemType;
  final int quantity;

  /// Ürün adı ve görsel backend'in sepet yanıtında yer almadığı için
  /// "Sepete Ekle" anında yerelde saklanan bir anlık görüntüdür (snapshot).
  final String title;
  final String imageUrl;

  /// Fiyat entegrasyonu ayrı bir görevde yapılacak; şimdilik her zaman `0`
  /// ("bilinmiyor" anlamında) — ekranlarda "Fiyat yakında" gösterilir.
  final double unitPrice;

  double get lineTotal => unitPrice * quantity;
}
