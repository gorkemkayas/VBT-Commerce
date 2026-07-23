import 'package:dio/dio.dart';

import '../../../../core/constants/app_constants.dart';

abstract interface class ProductPriceRemoteDataSource {
  Future<double> getPrice({
    required String sellableItemId,
    required String sellableItemType,
  });
}

/// `GET /api/prices/{sellableItemType}/{sellableItemId}` üzerinden anlık
/// fiyatı çeker — Cart feature'ının `PriceRemoteDataSource`'ı ile aynı
/// desen (bilinçli olarak ayrı bir instance; feature'lar arası data-layer
/// bağımlılığı kurulmuyor). Fiyat kaydı bulunamazsa (`404`) çağıran taraf
/// bunu ele alır — bu data source doğrudan hata fırlatır, "fiyat yok"
/// yorumunu yapmaz.
class ProductPriceRemoteDataSourceImpl implements ProductPriceRemoteDataSource {
  ProductPriceRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<double> getPrice({
    required String sellableItemId,
    required String sellableItemType,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '${AppConstants.productApiBaseUrl}/api/prices/$sellableItemType/$sellableItemId',
    );
    final amount = response.data?['amount'];
    if (amount is! num) {
      throw const FormatException('Fiyat bilgisi alınamadı.');
    }
    return amount.toDouble();
  }
}
