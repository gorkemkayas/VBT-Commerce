import 'package:freezed_annotation/freezed_annotation.dart';

import '../../domain/entities/product.dart';

part 'product_list_item_model.freezed.dart';
part 'product_list_item_model.g.dart';

/// `GET /api/products` yanıtındaki `ProductListItemDto` şekline karşılık gelir.
/// Bu DTO'da fiyat ve açıklama yok; fiyat, repository katmanında ayrı bir
/// sorgu (`GET /api/prices/{type}/{id}`) ile doldurulur.
@freezed
abstract class ProductListItemModel extends Product
    with _$ProductListItemModel {
  const ProductListItemModel._({
    required super.id,
    required super.title,
    required super.description,
    required super.category,
    required super.imageUrl,
    required super.hasVariants,
  });

  const factory ProductListItemModel({
    required String id,
    @JsonKey(name: 'name') required String title,
    @Default('') String description,
    @JsonKey(name: 'categoryId') required String category,
    @JsonKey(name: 'primaryImageUrl') @Default('') String imageUrl,
    @Default(false) bool hasVariants,
  }) = _ProductListItemModel;

  /// `hasVariants`, JSON'daki `productType` alanından (`"Simple"` |
  /// `"Variant"`) türetilir; sadece `"Variant"` ise ürünün gerçek
  /// varyantları vardır. `@JsonKey(fromJson:)` burada kullanılmıyor çünkü bu
  /// projenin kullandığı freezed dev sürümünde (`3.2.6-dev.1`) abstract
  /// freezed sınıflarında custom `fromJson` converter'ları es geçiliyor.
  factory ProductListItemModel.fromJson(Map<String, dynamic> json) =>
      _$ProductListItemModelFromJson({
        ...json,
        'hasVariants': json['productType'] == 'Variant',
      });
}
