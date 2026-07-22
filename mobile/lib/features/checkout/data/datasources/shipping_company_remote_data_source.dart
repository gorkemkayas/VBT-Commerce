import 'package:dio/dio.dart';

abstract interface class ShippingCompanyRemoteDataSource {
  /// `GET /api/shipping-companies` yalnızca aktif firmaları döner; seçim
  /// UI'ı henüz yok (ayrı bir görevde eklenecek), bu yüzden ilkinin id'sini
  /// döner. Liste boşsa `null` döner.
  Future<String?> getFirstActiveId();
}

class ShippingCompanyRemoteDataSourceImpl
    implements ShippingCompanyRemoteDataSource {
  ShippingCompanyRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<String?> getFirstActiveId() async {
    final response = await _dio.get<List<dynamic>>('/api/shipping-companies');
    final body = response.data ?? const [];
    for (final item in body) {
      if (item is Map<String, dynamic> && item['id'] is String) {
        return item['id'] as String;
      }
    }
    return null;
  }
}
