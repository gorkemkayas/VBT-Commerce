import 'product_variant.dart';

class Product {
  const Product({
    required this.id,
    required this.title,
    required this.description,
    required this.category,
    required this.imageUrl,
    this.imageUrls = const [],
    this.price,
    this.variants = const [],
    this.hasVariants = false,
  });

  final String id;
  final String title;
  final String description;
  final String category;
  final String imageUrl;

  /// Ürünün tüm görselleri, backend'in `DisplayOrder`'ına göre sıralı.
  /// Yalnızca ürün detayında (`GET /api/products/{id}`) doldurulur; ürün
  /// listesinde her zaman boştur (liste DTO'su yalnızca `primaryImageUrl`
  /// taşır, bkz. `ProductListItemModel`). `imageUrl` (tekil, birincil
  /// görsel) alanının anlamı değişmedi — cart/checkout/pricing hâlâ onu
  /// okur; bu alan tamamen ek (additive) bir gösterim kaynağıdır.
  final List<String> imageUrls;

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
