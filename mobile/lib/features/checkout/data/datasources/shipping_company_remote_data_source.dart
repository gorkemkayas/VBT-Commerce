import 'package:dio/dio.dart';

import '../models/shipping_company_model.dart';

abstract interface class ShippingCompanyRemoteDataSource {
  /// `GET /api/shipping-companies` — aktif kargo firmalarının tamamını döner.
  Future<List<ShippingCompanyModel>> getActive();
}

class ShippingCompanyRemoteDataSourceImpl
    implements ShippingCompanyRemoteDataSource {
  ShippingCompanyRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<List<ShippingCompanyModel>> getActive() async {
    final response = await _dio.get<List<dynamic>>('/api/shipping-companies');
    final body = response.data ?? const [];
    return body
        .whereType<Map<String, dynamic>>()
        .map(ShippingCompanyModel.fromJson)
        .toList(growable: false);
  }
}
