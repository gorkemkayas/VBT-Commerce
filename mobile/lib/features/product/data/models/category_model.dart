import '../../domain/entities/category.dart';

/// `GET /api/categories/tree` yanıtındaki `CategoryTreeDto` düğümüne karşılık
/// gelir. Yanıt bir ağaçtır (`children[]` iç içe kategoriler içerir); chip'ler
/// tek seviyeli bir liste beklediğinden [flattenTree] ile düzleştirilir. Böylece
/// hem üst hem alt kategoriler seçilebilir ve ürünlerin `categoryId`'siyle
/// birebir eşleşir.
class CategoryModel extends Category {
  const CategoryModel({required super.id, required super.name});

  factory CategoryModel.fromJson(Map<String, dynamic> json) => CategoryModel(
    id: json['id'] as String,
    name: json['name'] as String,
  );

  /// Ağacı ön-sıralı (parent önce, sonra çocukları) düz bir listeye çevirir.
  static List<CategoryModel> flattenTree(Object? json) {
    if (json is! List) {
      throw const FormatException('Kategori ağacı yanıtı beklenen şekilde değil.');
    }
    final result = <CategoryModel>[];
    void visit(Object? node) {
      if (node is! Map<String, dynamic>) return;
      result.add(CategoryModel.fromJson(node));
      final children = node['children'];
      if (children is List) {
        for (final child in children) {
          visit(child);
        }
      }
    }

    for (final node in json) {
      visit(node);
    }
    return result;
  }
}
