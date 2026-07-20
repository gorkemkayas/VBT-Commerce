import 'package:freezed_annotation/freezed_annotation.dart';

import '../../domain/entities/product.dart';

part 'product_list_item_model.freezed.dart';
part 'product_list_item_model.g.dart';

/// `GET /api/products` yanıtındaki `ProductListItemDto` şekline karşılık gelir.
/// Bu DTO'da fiyat ve açıklama yok; fiyat ayrı bir aşamada eklenecek.
@freezed
abstract class ProductListItemModel extends Product
    with _$ProductListItemModel {
  const ProductListItemModel._({
    required super.id,
    required super.title,
    required super.description,
    required super.category,
    required super.imageUrl,
  });

  const factory ProductListItemModel({
    required String id,
    @JsonKey(name: 'name') required String title,
    @Default('') String description,
    @JsonKey(name: 'categoryId') required String category,
    @JsonKey(name: 'primaryImageUrl') @Default('') String imageUrl,
  }) = _ProductListItemModel;

  factory ProductListItemModel.fromJson(Map<String, dynamic> json) =>
      _$ProductListItemModelFromJson(json);
}
