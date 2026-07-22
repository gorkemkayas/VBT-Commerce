import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_filter.dart';
import '../../domain/repositories/product_repository.dart';
import '../datasources/product_remote_data_source.dart';

class ProductRepositoryImpl implements ProductRepository {
  ProductRepositoryImpl(this._remoteDataSource);
  final ProductRemoteDataSource _remoteDataSource;
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
      return Result.success(await _remoteDataSource.getProductDetail(id));
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
    _cachedProducts = List.unmodifiable(products);
    return _cachedProducts!;
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
