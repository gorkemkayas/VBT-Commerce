import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/product_price_remote_data_source.dart';
import '../../data/datasources/product_remote_data_source.dart';
import '../../data/repositories/product_repository_impl.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_filter.dart';
import '../../domain/repositories/product_repository.dart';
import '../../domain/usecases/get_product_detail_use_case.dart';
import '../../domain/usecases/get_products_use_case.dart';
import '../../domain/usecases/filter_products_use_case.dart';
import '../../domain/usecases/get_categories_use_case.dart';
import '../../domain/usecases/get_variant_price_use_case.dart';
import '../../domain/usecases/search_products_use_case.dart';
import 'filter_state.dart';

final productRemoteDataSourceProvider = Provider<ProductRemoteDataSource>(
  (ref) => ProductRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final productPriceRemoteDataSourceProvider =
    Provider<ProductPriceRemoteDataSource>(
      (ref) => ProductPriceRemoteDataSourceImpl(ref.watch(dioProvider)),
    );
final productRepositoryProvider = Provider<ProductRepository>(
  (ref) => ProductRepositoryImpl(
    ref.watch(productRemoteDataSourceProvider),
    ref.watch(productPriceRemoteDataSourceProvider),
  ),
);
final getVariantPriceUseCaseProvider = Provider<GetVariantPriceUseCase>(
  (ref) => GetVariantPriceUseCase(ref.watch(productRepositoryProvider)),
);
final getProductsUseCaseProvider = Provider<GetProductsUseCase>(
  (ref) => GetProductsUseCase(ref.watch(productRepositoryProvider)),
);
final getProductDetailUseCaseProvider = Provider<GetProductDetailUseCase>(
  (ref) => GetProductDetailUseCase(ref.watch(productRepositoryProvider)),
);
final searchProductsUseCaseProvider = Provider<SearchProductsUseCase>(
  (ref) => SearchProductsUseCase(ref.watch(productRepositoryProvider)),
);
final getCategoriesUseCaseProvider = Provider<GetCategoriesUseCase>(
  (ref) => GetCategoriesUseCase(ref.watch(productRepositoryProvider)),
);

/// Kategori listesini bir kez yükleyip önbelleğe alır; kategori id'sini görünen
/// ada çevirmek isteyen ekranlar (ör. ürün detayı) bunu izler. Hata durumunda
/// boş liste döner, böylece tüketiciler id yerine hiçbir şey göstermez.
final categoriesProvider = FutureProvider<List<Category>>((ref) async {
  final result = await ref.watch(getCategoriesUseCaseProvider)();
  return switch (result) {
    Success<List<Category>>(:final value) => value,
    ResultFailure<List<Category>>() => const <Category>[],
  };
});
final filterProductsUseCaseProvider = Provider<FilterProductsUseCase>(
  (ref) => FilterProductsUseCase(ref.watch(productRepositoryProvider)),
);

class ProductListState {
  const ProductListState({
    this.isLoading = false,
    this.products = const [],
    this.failure,
  });
  final bool isLoading;
  final List<Product> products;
  final Failure? failure;
}

class ProductListController extends Notifier<ProductListState> {
  @override
  ProductListState build() => const ProductListState();

  Future<void> loadProducts() async {
    state = ProductListState(isLoading: true, products: state.products);
    final result = await ref.read(getProductsUseCaseProvider)();
    state = switch (result) {
      Success<List<Product>>(:final value) => ProductListState(products: value),
      ResultFailure<List<Product>>(:final failure) => ProductListState(
        failure: failure,
      ),
    };
  }
}

final productListControllerProvider =
    NotifierProvider<ProductListController, ProductListState>(
      ProductListController.new,
    );

final productDetailProvider = FutureProvider.autoDispose
    .family<Result<Product>, String>((ref, id) {
      return ref.watch(getProductDetailUseCaseProvider)(id);
    });

/// Kullanıcı, `Product.price`'ın temsil ettiği varsayılan (ilk) varyanttan
/// farklı bir varyant seçtiğinde o varyantın anlık fiyatını sorgular.
final variantPriceProvider = FutureProvider.autoDispose
    .family<Result<double?>, String>((ref, variantId) {
      return ref.watch(getVariantPriceUseCaseProvider)(variantId);
    });

class SearchFilterController extends Notifier<FilterState> {
  Timer? _debounce;
  int _requestVersion = 0;

  @override
  FilterState build() {
    ref.onDispose(() => _debounce?.cancel());
    Future.microtask(_initialize);
    return const FilterState(isLoading: true);
  }

  Future<void> _initialize() async {
    final categoriesResult = await ref.read(getCategoriesUseCaseProvider)();
    if (categoriesResult case Success<List<Category>>(:final value)) {
      state = state.copyWith(categories: value);
    }
    await _refreshResults();
  }

  void setQuery(String query) {
    state = state.copyWith(query: query, failure: null);
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 500), _refreshResults);
  }

  Future<void> setCategory(String? category) async {
    state = state.copyWith(category: category, failure: null);
    await _refreshResults();
  }

  Future<void> setPriceRange({double? minPrice, double? maxPrice}) async {
    state = state.copyWith(
      minPrice: minPrice,
      maxPrice: maxPrice,
      failure: null,
    );
    await _refreshResults();
  }

  Future<void> setSort(ProductSortOption sort) async {
    state = state.copyWith(sort: sort, failure: null);
    await _refreshResults();
  }

  Future<void> clear() async {
    _debounce?.cancel();
    state = FilterState(categories: state.categories, isLoading: true);
    await _refreshResults();
  }

  Future<void> _refreshResults() async {
    final version = ++_requestVersion;
    state = state.copyWith(isLoading: true, failure: null);
    final filter = state.filter;
    final hasOnlyQuery =
        filter.category == null &&
        filter.minPrice == null &&
        filter.maxPrice == null &&
        filter.sort == ProductSortOption.none;
    final result = hasOnlyQuery
        ? await ref.read(searchProductsUseCaseProvider)(filter.query)
        : await ref.read(filterProductsUseCaseProvider)(filter);
    if (version != _requestVersion) return;
    state = switch (result) {
      Success<List<Product>>(:final value) => state.copyWith(
        products: value,
        isLoading: false,
      ),
      ResultFailure<List<Product>>(:final failure) => state.copyWith(
        isLoading: false,
        failure: failure,
      ),
    };
  }
}

final searchFilterControllerProvider =
    NotifierProvider<SearchFilterController, FilterState>(
      SearchFilterController.new,
    );
