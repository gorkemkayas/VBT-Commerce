import 'product_variant.dart';

class Product {
  const Product({
    required this.id,
    required this.title,
    required this.description,
    required this.category,
    required this.imageUrl,
    this.price,
    this.variants = const [],
  });

  final String id;
  final String title;
  final String description;
  final String category;
  final String imageUrl;

  /// Bu entegrasyon aşamasında fiyat endpoint'i kullanılmıyor, bu yüzden
  /// her zaman null döner. Fiyat entegrasyonu ayrı bir aşamada eklenecek.
  final double? price;

  /// Ürünün satılabilir varyantları (ör. beden seçenekleri). Yalnızca ürün
  /// detayında (`GET /api/products/{id}`) doldurulur; ürün listesinde bu
  /// bilgi yoktur, bu yüzden liste öğelerinde her zaman boştur. Boşsa ürünün
  /// varyantı yok demektir — sepete kendi id'siyle eklenir.
  final List<ProductVariant> variants;
}
