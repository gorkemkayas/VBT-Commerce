import '../../domain/entities/favorite_item.dart';

/// [FavoriteItem]'ın yerel depolamaya (SharedPreferences) JSON olarak
/// serileştirilebilir hâli.
class FavoriteItemModel extends FavoriteItem {
  const FavoriteItemModel({
    required super.productId,
    required super.title,
    required super.imageUrl,
  });

  factory FavoriteItemModel.fromEntity(FavoriteItem item) => FavoriteItemModel(
    productId: item.productId,
    title: item.title,
    imageUrl: item.imageUrl,
  );

  factory FavoriteItemModel.fromJson(Map<String, dynamic> json) =>
      FavoriteItemModel(
        productId: json['productId'] as String,
        title: json['title'] as String,
        imageUrl: json['imageUrl'] as String? ?? '',
      );

  Map<String, dynamic> toJson() => {
    'productId': productId,
    'title': title,
    'imageUrl': imageUrl,
  };
}
