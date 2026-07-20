import 'package:freezed_annotation/freezed_annotation.dart';

import '../../domain/entities/product.dart';

part 'product_detail_model.freezed.dart';
part 'product_detail_model.g.dart';

/// `GET /api/products/{productId}` yanıtındaki `ProductDto` şekline karşılık
/// gelir. `imageUrl`, `images[]` içindeki `isPrimary: true` olan görselden
/// (yoksa ilk görselden) türetilir. `variantId`, `variants[]` içindeki aktif
/// varyanttan (yoksa ilk varyanttan) türetilir — sepete eklerken kullanılan
/// `sellableItemId` budur. Fiyat bu DTO'da da yok.
ProductDetailModel _productDetailModelFromJson(Map<String, dynamic> json) {
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
  final variants = json['variants'];
  String? variantId;
  if (variants is List && variants.isNotEmpty) {
    final active = variants.firstWhere(
      (variant) => variant is Map && variant['isActive'] == true,
      orElse: () => variants.first,
    );
    if (active is Map && active['id'] is String) {
      variantId = active['id'] as String;
    }
  }
  return ProductDetailModel(
    id: json['id'] as String,
    title: json['name'] as String,
    description: json['description'] as String? ?? '',
    category: json['categoryId'] as String,
    imageUrl: imageUrl,
    variantId: variantId,
  );
}

@freezed
abstract class ProductDetailModel extends Product with _$ProductDetailModel {
  const ProductDetailModel._({
    required super.id,
    required super.title,
    required super.description,
    required super.category,
    required super.imageUrl,
    super.variantId,
  });

  const factory ProductDetailModel({
    required String id,
    required String title,
    required String description,
    required String category,
    required String imageUrl,
    String? variantId,
  }) = _ProductDetailModel;

  factory ProductDetailModel.fromJson(Map<String, dynamic> json) =>
      _productDetailModelFromJson(json);
}
