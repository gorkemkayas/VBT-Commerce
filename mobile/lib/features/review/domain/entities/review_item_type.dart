/// Backend'deki `Review.Domain.Enums.ReviewItemType` ile birebir eşleşir.
/// Bir değerlendirme her zaman kullanıcının satın aldığı tam kaleme
/// (ürünün kendisi ya da seçtiği varyant) bağlanır — her zaman üst ürüne
/// yuvarlanmaz (bkz. `ProductReview` entity'sindeki backend yorumu).
enum ReviewItemType {
  product,
  variant;

  /// Backend enum'ları `JsonStringEnumConverter` ile string olarak
  /// serileştirilir/çözümlenir; sorgu parametrelerinde de aynı isimler
  /// (`Product`/`Variant`) kullanılır.
  String toJson() => switch (this) {
    ReviewItemType.product => 'Product',
    ReviewItemType.variant => 'Variant',
  };
}

ReviewItemType reviewItemTypeFromJson(String value) => switch (value) {
  'Product' => ReviewItemType.product,
  'Variant' => ReviewItemType.variant,
  _ => throw FormatException('Bilinmeyen inceleme öğesi türü: $value'),
};
