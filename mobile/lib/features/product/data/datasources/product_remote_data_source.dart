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

  @override
  Future<List<ProductListItemModel>> getProducts() async {
    final response = await _dio.get<Map<String, dynamic>>(
      '${AppConstants.productApiBaseUrl}/api/products',
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş ürün listesi alındı.');
    }
    final items = body['items'];
    if (items is! List) {
      throw const FormatException(
        'Ürün listesi yanıtı beklenen şekilde değil.',
      );
    }
    return items
        .map(
          (item) => ProductListItemModel.fromJson(item as Map<String, dynamic>),
        )
        .toList(growable: false);
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
