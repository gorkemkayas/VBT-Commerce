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
