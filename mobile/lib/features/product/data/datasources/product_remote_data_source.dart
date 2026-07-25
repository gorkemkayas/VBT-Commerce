import 'package:dio/dio.dart';

import '../../../../core/constants/app_constants.dart';
import '../models/category_model.dart';
import '../models/product_detail_model.dart';
import '../models/product_list_item_model.dart';

abstract interface class ProductRemoteDataSource {
  Future<List<ProductListItemModel>> getProducts();
  Future<ProductDetailModel> getProductDetail(String id);
  Future<List<CategoryModel>> getCategories();
}

class ProductRemoteDataSourceImpl implements ProductRemoteDataSource {
  ProductRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  /// Backend `pageSize` için 1-100 aralığını kabul ediyor (bkz.
  /// `GetProductsListQueryValidator`); tek istekte alınabilecek en büyük
  /// sayfa boyutu.
  static const _pageSize = 100;

  /// Aşırı büyük bir katalogda sonsuz döngüye girmemek için güvenlik sınırı
  /// (50 sayfa × 100 = 5000 ürün).
  static const _maxPages = 50;

  @override
  Future<List<ProductListItemModel>> getProducts() async {
    final items = <ProductListItemModel>[];
    var pageNumber = 1;
    while (pageNumber <= _maxPages) {
      final response = await _dio.get<Map<String, dynamic>>(
        '${AppConstants.productApiBaseUrl}/api/products',
        queryParameters: {'pageNumber': pageNumber, 'pageSize': _pageSize},
      );
      final body = response.data;
      if (body == null) {
        throw const FormatException('Sunucudan boş ürün listesi alındı.');
      }
      final pageItems = body['items'];
      if (pageItems is! List) {
        throw const FormatException(
          'Ürün listesi yanıtı beklenen şekilde değil.',
        );
      }
      items.addAll(
        pageItems.map(
          (item) => ProductListItemModel.fromJson(item as Map<String, dynamic>),
        ),
      );
      final totalCount = (body['totalCount'] as num?)?.toInt();
      final reachedEnd =
          pageItems.isEmpty ||
          pageItems.length < _pageSize ||
          (totalCount != null && items.length >= totalCount);
      if (reachedEnd) break;
      pageNumber++;
    }
    return items;
  }

  @override
  Future<ProductDetailModel> getProductDetail(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '${AppConstants.productApiBaseUrl}/api/products/$id',
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş ürün detayı alındı.');
    }
    return ProductDetailModel.fromJson(body);
  }

  @override
  Future<List<CategoryModel>> getCategories() async {
    final response = await _dio.get<List<dynamic>>(
      '${AppConstants.productApiBaseUrl}/api/categories/tree',
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş kategori listesi alındı.');
    }
    return CategoryModel.flattenTree(body);
  }
}
