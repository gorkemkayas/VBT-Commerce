/// Bir ürünün satılabilir varyantı (ör. beden ya da renk seçeneği).
class ProductVariant {
  const ProductVariant({
    required this.id,
    required this.size,
    this.color = '',
    this.imageUrls = const [],
  });

  final String id;

  /// "Beden" özniteliğinin değeri (ör. "M", "XL"); beden taşımayan
  /// varyantlarda boş string.
  final String size;

  /// "Renk" özniteliğinin değeri — backend'de genellikle hex kod olarak
  /// gelir (ör. "#000000"); renk taşımayan varyantlarda boş string.
  final String color;

  /// Bu varyanta özel görseller (`ProductImageDto.ProductVariantId` bu
  /// varyantın id'sine eşit olanlar), `displayOrder`'a göre sıralı. Boşsa bu
  /// varyantın kendine özel görseli yok demektir — ürünün genel
  /// görselleri (`Product.imageUrls`) gösterilir.
  final List<String> imageUrls;
}
