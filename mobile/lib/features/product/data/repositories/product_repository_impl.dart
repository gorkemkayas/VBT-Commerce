import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_filter.dart';
import '../../domain/repositories/product_repository.dart';
import '../datasources/product_price_remote_data_source.dart';
import '../datasources/product_remote_data_source.dart';

class ProductRepositoryImpl implements ProductRepository {
  ProductRepositoryImpl(this._remoteDataSource, this._priceDataSource);
  final ProductRemoteDataSource _remoteDataSource;
  final ProductPriceRemoteDataSource _priceDataSource;
  List<Product>? _cachedProducts;

  @override
  Future<Result<List<Product>>> getProducts() async {
    try {
      final products = await _getProductsFromCacheOrRemote();
      return Result.success(products);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Ürünler alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<Product>> getProductDetail(String id) async {
    try {
      final detail = await _remoteDataSource.getProductDetail(id);
      return Result.success(await _withPrice(detail));
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Ürün detayı alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<double?>> getVariantPrice(String variantId) async {
    return Result.success(
      await _resolvePrice(sellableItemId: variantId, sellableItemType: 'Variant'),
    );
  }

  @override
  Future<Result<List<Category>>> getCategories() async {
    try {
      final categories = await _remoteDataSource.getCategories();
      return Result.success(categories);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Kategoriler alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<List<Product>>> searchProducts(String query) =>
      filterProducts(ProductFilter(query: query));

  @override
  Future<Result<List<Product>>> filterProducts(ProductFilter filter) async {
    try {
      var products = await _getProductsFromCacheOrRemote();
      final query = filter.query.trim().toLowerCase();
      if (query.isNotEmpty) {
        products = products
            .where((product) => product.title.toLowerCase().contains(query))
            .toList(growable: false);
      }
      if (filter.category != null) {
        products = products
            .where((product) => product.category == filter.category)
            .toList(growable: false);
      }
      if (filter.minPrice != null) {
        products = products
            .where((product) => (product.price ?? 0) >= filter.minPrice!)
            .toList(growable: false);
      }
      if (filter.maxPrice != null) {
        products = products
            .where((product) => (product.price ?? 0) <= filter.maxPrice!)
            .toList(growable: false);
      }
      final sorted = [...products]
        ..sort((first, second) => _compare(first, second, filter.sort));
      return Result.success(sorted);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Ürünler filtrelenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  Future<List<Product>> _getProductsFromCacheOrRemote() async {
    final cached = _cachedProducts;
    if (cached != null) return cached;
    final products = await _remoteDataSource.getProducts();
    final priced = await Future.wait(products.map(_withPrice));
    _cachedProducts = List.unmodifiable(priced);
    return _cachedProducts!;
  }

  Future<Product> _withPrice(Product product) async {
    final reference = _priceReferenceFor(product);
    final price = reference == null
        ? null
        : await _resolvePrice(
            sellableItemId: reference.id,
            sellableItemType: reference.type,
          );
    return Product(
      id: product.id,
      title: product.title,
      description: product.description,
      category: product.category,
      imageUrl: product.imageUrl,
      price: price,
      variants: product.variants,
      hasVariants: product.hasVariants,
    );
  }

  /// Varyantlı bir üründe fiyat, ilk varyant üzerinden sorgulanır — Cart'ın
  /// sepete eklerken kullandığı kuralla aynı (varyantlı ürünlerde `Variant`
  /// tipi + varyant id'si, aksi halde `Product` tipi + ürün id'si).
  ///
  /// Ürün listesindeki öğelerde `hasVariants` doğru gelir (`productType`'tan
  /// türetilir) ama gerçek `variants[]` hiç gelmez (bkz. `Product.variants`
  /// dokümantasyonu) — yani "ilk varyant"ın id'si bilinmez. Bu durumda yanlış
  /// (`Product` tipi) bir fiyat sorgulamak yerine `null` dönülür; doğru fiyat,
  /// ilgili ürün kartı `getProductDetail` akışını tetiklediğinde ayrıca
  /// çözülür (bkz. `ProductCard`) — bu sayede List, Detail ve Cart her zaman
  /// aynı `sellableItemType`+id kombinasyonuna varır.
  ({String id, String type})? _priceReferenceFor(Product product) {
    if (!product.hasVariants) {
      return (id: product.id, type: 'Product');
    }
    if (product.variants.isEmpty) {
      return null;
    }
    return (id: product.variants.first.id, type: 'Variant');
  }

  /// Fiyat endpoint'i başarısız olursa (`404` dahil) ürün akışı kırılmaz;
  /// `null` döner ve ekranda "Fiyat yakında" gösterilmeye devam eder.
  Future<double?> _resolvePrice({
    required String sellableItemId,
    required String sellableItemType,
  }) async {
    try {
      return await _priceDataSource.getPrice(
        sellableItemId: sellableItemId,
        sellableItemType: sellableItemType,
      );
    } catch (_) {
      return null;
    }
  }

  int _compare(Product first, Product second, ProductSortOption sort) =>
      switch (sort) {
        ProductSortOption.none => 0,
        ProductSortOption.priceAscending => (first.price ?? 0).compareTo(
          second.price ?? 0,
        ),
        ProductSortOption.priceDescending => (second.price ?? 0).compareTo(
          first.price ?? 0,
        ),
      };
}
