/// Bir ürün kategorisi. `id` filtreleme için kullanılır (ürünün `category`
/// alanı da bu id'yi tutar), `name` ise kullanıcıya gösterilen etikettir.
class Category {
  const Category({required this.id, required this.name});

  final String id;
  final String name;

  @override
  bool operator ==(Object other) =>
      other is Category && other.id == id && other.name == name;

  @override
  int get hashCode => Object.hash(id, name);
}

/// `GET /api/categories/{categoryId}` ile dönen tek kategori kaydı. Kategori
/// ağacındaki (`Category`) id+ad ikilisine ek olarak açıklama, görsel ve
/// üst kategori bilgisini taşır; ürün detayındaki kategori rozeti bunu
/// kullanır. `isActive`, ağaçta görünmeyen (pasife alınmış) bir kategorinin
/// adının yine de gösterilebilmesi için okunur.
class CategoryDetail extends Category {
  const CategoryDetail({
    required super.id,
    required super.name,
    required this.slug,
    this.description,
    this.imageUrl,
    this.parentCategoryId,
    this.isActive = true,
  });

  final String slug;
  final String? description;
  final String? imageUrl;
  final String? parentCategoryId;
  final bool isActive;
}
