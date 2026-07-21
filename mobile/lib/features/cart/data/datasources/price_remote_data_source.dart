import 'package:dio/dio.dart';

abstract interface class PriceRemoteDataSource {
  Future<double> getPrice({
    required String sellableItemId,
    required String sellableItemType,
  });
}

/// `GET /api/prices/{sellableItemType}/{sellableItemId}` üzerinden anlık
/// fiyatı çeker. Fiyat kaydı bulunamazsa (`404`) çağıran taraf bunu ele alır —
/// bu data source doğrudan hata fırlatır, "fiyat yok" yorumunu yapmaz.
class PriceRemoteDataSourceImpl implements PriceRemoteDataSource {
  PriceRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<double> getPrice({
    required String sellableItemId,
    required String sellableItemType,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/prices/$sellableItemType/$sellableItemId',
    );
    final amount = response.data?['amount'];
    if (amount is! num) {
      throw const FormatException('Fiyat bilgisi alınamadı.');
    }
    return amount.toDouble();
  }
}
