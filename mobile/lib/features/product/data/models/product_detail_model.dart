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
    super.imageUrls,
    super.variants,
    super.hasVariants,
  });

  factory ProductDetailModel.fromJson(Map<String, dynamic> json) {
    final images = _parsedImages(json['images']);

    var imageUrl = '';
    if (images.isNotEmpty) {
      final primary = images.firstWhere(
        (image) => image['isPrimary'] == true,
        orElse: () => images.first,
      );
      final url = primary['url'];
      if (url is String) imageUrl = url;
    }

    // Belirli bir varyanta bağlı olmayan (ürünün genel) görseller.
    final imageUrls = images
        .where((image) => image['productVariantId'] == null)
        .map((image) => image['url'] as String)
        .toList(growable: false);

    final variants = _variantsFromJson(json['variants'], images);
    return ProductDetailModel(
      id: json['id'] as String,
      title: json['name'] as String,
      description: json['description'] as String? ?? '',
      category: json['categoryId'] as String,
      imageUrl: imageUrl,
      imageUrls: imageUrls,
      variants: variants,
      hasVariants: variants.isNotEmpty,
    );
  }
}

/// `images[]` dizisini `displayOrder`'a göre sıralı, geçerli (URL'si olan)
/// haritalar listesine çevirir. Hem genel (`imageUrl`/`Product.imageUrls`)
/// hem varyanta özel (`ProductVariant.imageUrls`) görseller bunun üzerinden
/// türetilir.
List<Map> _parsedImages(Object? json) {
  if (json is! List) return const [];
  final images = json
      .whereType<Map>()
      .where((image) => image['url'] is String)
      .toList();
  images.sort((first, second) {
    final firstOrder = (first['displayOrder'] as num?)?.toInt() ?? 0;
    final secondOrder = (second['displayOrder'] as num?)?.toInt() ?? 0;
    return firstOrder.compareTo(secondOrder);
  });
  return images;
}

List<ProductVariant> _variantsFromJson(Object? json, List<Map> images) {
  if (json is! List) return const [];
  final variants = <ProductVariant>[];
  for (final item in json) {
    if (item is! Map) continue;
    final id = item['id'];
    if (id is! String) continue;
    var size = '';
    var color = '';
    final optionValues = item['optionValues'];
    if (optionValues is List) {
      for (final option in optionValues) {
        if (option is! Map) continue;
        final value = option['value'];
        if (value is! String) continue;
        switch (option['attributeName']) {
          case 'Beden':
            size = value;
          case 'Renk':
            color = value;
        }
      }
    }
    final variantImageUrls = images
        .where((image) => image['productVariantId'] == id)
        .map((image) => image['url'] as String)
        .toList(growable: false);
    variants.add(
      ProductVariant(
        id: id,
        size: size,
        color: color,
        imageUrls: variantImageUrls,
      ),
    );
  }
  return variants;
}
