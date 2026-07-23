import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../../product/domain/entities/category.dart';
import '../../../product/domain/entities/product.dart';
import '../../../product/domain/entities/product_filter.dart';
import '../../../product/presentation/providers/product_providers.dart';

const _unset = Object();

/// Ana sayfa yalnızca bir sunum katmanı bileşimidir: kendi domain/data
/// katmanı yoktur, `product` özelliğinin use case'lerini yeniden kullanır.
class HomeState {
  const HomeState({
    this.isLoading = false,
    this.categories = const [],
    this.featuredProducts = const [],
    this.recommendedProducts = const [],
    this.selectedCategory,
    this.failure,
  });

  final bool isLoading;
  final List<Category> categories;
  final List<Product> featuredProducts;
  final List<Product> recommendedProducts;
  final String? selectedCategory;
  final Failure? failure;

  bool get isEmpty => featuredProducts.isEmpty && recommendedProducts.isEmpty;

  /// Seçili kategorinin (id) görünen adı; hiçbiri seçili değilse `null`.
  /// Liste henüz yüklenmemişse eşleşme bulunamayabilir, bu durumda da `null`.
  String? get selectedCategoryName {
    final id = selectedCategory;
    if (id == null) return null;
    for (final category in categories) {
      if (category.id == id) return category.name;
    }
    return null;
  }

  HomeState copyWith({
    bool? isLoading,
    List<Category>? categories,
    List<Product>? featuredProducts,
    List<Product>? recommendedProducts,
    Object? selectedCategory = _unset,
    Object? failure = _unset,
  }) => HomeState(
    isLoading: isLoading ?? this.isLoading,
    categories: categories ?? this.categories,
    featuredProducts: featuredProducts ?? this.featuredProducts,
    recommendedProducts: recommendedProducts ?? this.recommendedProducts,
    selectedCategory: identical(selectedCategory, _unset)
        ? this.selectedCategory
        : selectedCategory as String?,
    failure: identical(failure, _unset) ? this.failure : failure as Failure?,
  );
}

class HomeController extends Notifier<HomeState> {
  static const _featuredCount = 6;

  @override
  HomeState build() => const HomeState();

  Future<void> load() async {
    state = state.copyWith(isLoading: true, failure: null);

    final categoriesResult = await ref.read(getCategoriesUseCaseProvider)();
    final productsResult = await ref.read(getProductsUseCaseProvider)();

    final categories = switch (categoriesResult) {
      Success<List<Category>>(:final value) => value,
      ResultFailure<List<Category>>() => const <Category>[],
    };

    state = switch (productsResult) {
      Success<List<Product>>(:final value) => state.copyWith(
        isLoading: false,
        categories: categories,
        featuredProducts: value.take(_featuredCount).toList(growable: false),
        recommendedProducts: value.length > _featuredCount
            ? value.skip(_featuredCount).toList(growable: false)
            : value,
        selectedCategory: null,
        failure: null,
      ),
      ResultFailure<List<Product>>(:final failure) => state.copyWith(
        isLoading: false,
        failure: failure,
      ),
    };
  }

  /// `null` kategori "Tümü" seçimidir ve tam listeyi geri yükler.
  Future<void> selectCategory(String? category) async {
    if (category == null) {
      await load();
      return;
    }

    state = state.copyWith(
      selectedCategory: category,
      isLoading: true,
      failure: null,
    );

    final result = await ref.read(filterProductsUseCaseProvider)(
      ProductFilter(category: category),
    );

    state = switch (result) {
      Success<List<Product>>(:final value) => state.copyWith(
        isLoading: false,
        recommendedProducts: value,
        failure: null,
      ),
      ResultFailure<List<Product>>(:final failure) => state.copyWith(
        isLoading: false,
        failure: failure,
      ),
    };
  }
}

final homeControllerProvider = NotifierProvider<HomeController, HomeState>(
  HomeController.new,
);
