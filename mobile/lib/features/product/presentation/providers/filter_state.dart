import '../../../../core/errors/failure.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_filter.dart';

const _unset = Object();

class FilterState {
  const FilterState({
    this.query = '',
    this.category,
    this.minPrice,
    this.maxPrice,
    this.sort = ProductSortOption.none,
    this.categories = const [],
    this.products = const [],
    this.isLoading = false,
    this.failure,
  });

  final String query;
  final String? category;
  final double? minPrice;
  final double? maxPrice;
  final ProductSortOption sort;
  final List<String> categories;
  final List<Product> products;
  final bool isLoading;
  final Failure? failure;

  ProductFilter get filter => ProductFilter(
    query: query,
    category: category,
    minPrice: minPrice,
    maxPrice: maxPrice,
    sort: sort,
  );

  FilterState copyWith({
    String? query,
    Object? category = _unset,
    Object? minPrice = _unset,
    Object? maxPrice = _unset,
    ProductSortOption? sort,
    List<String>? categories,
    List<Product>? products,
    bool? isLoading,
    Object? failure = _unset,
  }) => FilterState(
    query: query ?? this.query,
    category: identical(category, _unset) ? this.category : category as String?,
    minPrice: identical(minPrice, _unset) ? this.minPrice : minPrice as double?,
    maxPrice: identical(maxPrice, _unset) ? this.maxPrice : maxPrice as double?,
    sort: sort ?? this.sort,
    categories: categories ?? this.categories,
    products: products ?? this.products,
    isLoading: isLoading ?? this.isLoading,
    failure: identical(failure, _unset) ? this.failure : failure as Failure?,
  );
}
