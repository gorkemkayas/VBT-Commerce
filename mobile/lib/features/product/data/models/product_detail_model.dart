import '../../domain/entities/product.dart';
import '../../domain/entities/product_variant.dart';

/// `GET /api/products/{productId}` yanıtındaki `ProductDto` şekline karşılık
/// gelir. `imageUrl`, `images[]` içindeki `isPrimary: true` olan görselden
/// (yoksa ilk görselden) türetilir. `variants`, `variants[]` içindeki her
/// varyantın id'si ve "Beden" seçenek değeriyle oluşturulur. Fiyat bu DTO'da
/// da yok.
///
/// Freezed yerine düz bir sınıf olarak tanımlanır — `variants` alanı
/// entity'den miras alınan `List<ProductVariant>` tipiyle birebir aynı
/// olduğundan Freezed'in otomatik equality kod üretimi bu alanda derleme
/// hatası veriyor (bkz. `CustomerModel`'de aynı nedenle alınan karar).
class ProductDetailModel extends Product {
  const ProductDetailModel({
    required super.id,
    required super.title,
    required super.description,
    required super.category,
    required super.imageUrl,
    super.variants,
  });

  factory ProductDetailModel.fromJson(Map<String, dynamic> json) {
    final images = json['images'];
    var imageUrl = '';
    if (images is List && images.isNotEmpty) {
      final primary = images.firstWhere(
        (image) => image is Map && image['isPrimary'] == true,
        orElse: () => images.first,
      );
      if (primary is Map && primary['url'] is String) {
        imageUrl = primary['url'] as String;
      }
    }
    return ProductDetailModel(
      id: json['id'] as String,
      title: json['name'] as String,
      description: json['description'] as String? ?? '',
      category: json['categoryId'] as String,
      imageUrl: imageUrl,
      variants: _variantsFromJson(json['variants']),
    );
  }
}

List<ProductVariant> _variantsFromJson(Object? json) {
  if (json is! List) return const [];
  final variants = <ProductVariant>[];
  for (final item in json) {
    if (item is! Map) continue;
    final id = item['id'];
    if (id is! String) continue;
    var size = '';
    final optionValues = item['optionValues'];
    if (optionValues is List) {
      for (final option in optionValues) {
        if (option is Map && option['attributeName'] == 'Beden') {
          final value = option['value'];
          if (value is String) size = value;
          break;
        }
      }
    }
    variants.add(ProductVariant(id: id, size: size));
  }
  return variants;
}
