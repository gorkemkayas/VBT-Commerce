import 'package:dio/dio.dart';

import '../models/order_model.dart';

abstract interface class OrderRemoteDataSource {
  Future<List<OrderModel>> getMyOrders();
}

/// `GET /api/orders/me`. Yanıt sayfalıdır (`PagedResult`): siparişler `items`
/// dizisindedir. Sayfalama UI'ı henüz yok, bu yüzden ilk sayfa yeterince büyük
/// bir `pageSize` ile çekilir.
class OrderRemoteDataSourceImpl implements OrderRemoteDataSource {
  OrderRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  static const _pageSize = 50;

  @override
  Future<List<OrderModel>> getMyOrders() async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/orders/me',
      queryParameters: {'pageNumber': 1, 'pageSize': _pageSize},
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş sipariş listesi alındı.');
    }
    final items = body['items'];
    if (items is! List) {
      throw const FormatException(
        'Sipariş listesi yanıtı beklenen şekilde değil.',
      );
    }
    return items
        .map((item) => OrderModel.fromJson(item as Map<String, dynamic>))
        .toList(growable: false);
  }
}
