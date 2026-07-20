import 'package:dio/dio.dart';

import 'cart_remote_data_source.dart' show cartSellableItemType;

abstract interface class PriceRemoteDataSource {
  Future<double> getPrice(String sellableItemId);
}

/// `GET /api/prices/{sellableItemType}/{sellableItemId}` üzerinden anlık
/// fiyatı çeker — bu fiyat, "Sepete Ekle" anında snapshot olarak saklanır.
class PriceRemoteDataSourceImpl implements PriceRemoteDataSource {
  PriceRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<double> getPrice(String sellableItemId) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/prices/$cartSellableItemType/$sellableItemId',
    );
    final amount = response.data?['amount'];
    if (amount is! num) {
      throw const FormatException('Fiyat bilgisi alınamadı.');
    }
    return amount.toDouble();
  }
}
