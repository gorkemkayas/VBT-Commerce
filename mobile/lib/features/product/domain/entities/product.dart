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
    this.hasVariants = false,
  });

  final String id;
  final String title;
  final String description;
  final String category;
  final String imageUrl;

  /// Ürün DTO'larında (liste/detay) yer almaz; repository katmanında ayrı bir
  /// sorgu (`GET /api/prices/{type}/{id}`) ile doldurulur. Fiyat kaydı yoksa
  /// veya sorgu başarısız olursa `null` kalır ve UI "Fiyat yakında" gösterir.
  final double? price;

  /// Ürünün satılabilir varyantları (ör. beden seçenekleri). Yalnızca ürün
  /// detayında (`GET /api/products/{id}`) doldurulur; ürün listesinde bu
  /// bilgi yoktur, bu yüzden liste öğelerinde her zaman boştur. Boşsa ürünün
  /// varyantı yok demektir — sepete kendi id'siyle eklenir.
  final List<ProductVariant> variants;

  /// Ürünün varyantlı olup olmadığı (backend'deki `ProductType == Variant`).
  /// Liste öğelerinde `variants` her zaman boş geldiği için varyantlı
  /// ürünleri ayırt etmek amacıyla ayrıca taşınır; detay modelinde
  /// `variants.isNotEmpty` ile aynı bilgiyi verir.
  final bool hasVariants;
}
