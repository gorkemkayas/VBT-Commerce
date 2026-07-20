enum ProductSortOption { none, priceAscending, priceDescending }

class ProductFilter {
  const ProductFilter({
    this.query = '',
    this.category,
    this.minPrice,
    this.maxPrice,
    this.sort = ProductSortOption.none,
  });

  final String query;
  final String? category;
  final double? minPrice;
  final double? maxPrice;
  final ProductSortOption sort;
}
