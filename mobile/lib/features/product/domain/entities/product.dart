class Product {
  const Product({
    required this.id,
    required this.title,
    required this.description,
    required this.category,
    required this.imageUrl,
    this.price,
    this.variantId,
  });

  final String id;
  final String title;
  final String description;
  final String category;
  final String imageUrl;

  /// Bu entegrasyon aşamasında fiyat endpoint'i kullanılmıyor, bu yüzden
  /// her zaman null döner. Fiyat entegrasyonu ayrı bir aşamada eklenecek.
  final double? price;

  /// Sepete eklenebilecek satılabilir varyantın kimliği. Yalnızca ürün
  /// detayında (`GET /api/products/{id}`) doldurulur; ürün listesinde bu
  /// bilgi yoktur, bu yüzden liste öğelerinde her zaman null'dur. Ürünün
  /// aktif bir varyantı yoksa da null kalır — bu durumda sepete eklenemez.
  final String? variantId;
}
