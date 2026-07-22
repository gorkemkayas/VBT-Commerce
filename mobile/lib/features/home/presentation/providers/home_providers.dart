import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../../product/domain/entities/product.dart';
import '../../../product/presentation/providers/product_providers.dart';

const _unset = Object();

/// Ana sayfa yalnızca bir sunum katmanı bileşimidir: kendi domain/data
/// katmanı yoktur, `product` özelliğinin use case'lerini yeniden kullanır.
///
/// Kategori seçimi ana sayfada değil, "Koleksiyonu keşfet" ile açılan ürün
/// listesi ekranındadır (bkz. `ProductListController`).
class HomeState {
  const HomeState({
    this.isLoading = false,
    this.featuredProducts = const [],
    this.recommendedProducts = const [],
    this.failure,
  });

  final bool isLoading;
  final List<Product> featuredProducts;
  final List<Product> recommendedProducts;
  final Failure? failure;

  bool get isEmpty => featuredProducts.isEmpty && recommendedProducts.isEmpty;

  HomeState copyWith({
    bool? isLoading,
    List<Product>? featuredProducts,
    List<Product>? recommendedProducts,
    Object? failure = _unset,
  }) => HomeState(
    isLoading: isLoading ?? this.isLoading,
    featuredProducts: featuredProducts ?? this.featuredProducts,
    recommendedProducts: recommendedProducts ?? this.recommendedProducts,
    failure: identical(failure, _unset) ? this.failure : failure as Failure?,
  );
}

class HomeController extends Notifier<HomeState> {
  static const _featuredCount = 6;

  @override
  HomeState build() => const HomeState();

  Future<void> load() async {
    state = state.copyWith(isLoading: true, failure: null);

    final productsResult = await ref.read(getProductsUseCaseProvider)();

    state = switch (productsResult) {
      Success<List<Product>>(:final value) => state.copyWith(
        isLoading: false,
        featuredProducts: value.take(_featuredCount).toList(growable: false),
        recommendedProducts: value.length > _featuredCount
            ? value.skip(_featuredCount).toList(growable: false)
            : value,
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
