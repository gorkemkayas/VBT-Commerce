// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'product_list_item_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ProductListItemModel _$ProductListItemModelFromJson(
  Map<String, dynamic> json,
) => _ProductListItemModel(
  id: json['id'] as String,
  title: json['name'] as String,
  description: json['description'] as String? ?? '',
  category: json['categoryId'] as String,
  imageUrl: json['primaryImageUrl'] as String? ?? '',
);

Map<String, dynamic> _$ProductListItemModelToJson(
  _ProductListItemModel instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.title,
  'description': instance.description,
  'categoryId': instance.category,
  'primaryImageUrl': instance.imageUrl,
};
