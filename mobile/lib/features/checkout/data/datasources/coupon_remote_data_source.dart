import 'package:dio/dio.dart';

import '../models/coupon_model.dart';

abstract interface class CouponRemoteDataSource {
  /// `GET /api/coupons/active` — vitrinde gösterilebilen aktif kuponlar.
  /// `CouponsController` içindeki tek herkese açık uç (diğerleri
  /// `/api/admin/coupons` altında ve rol istiyor), bu yüzden oturum
  /// gerektirmez.
  Future<List<CouponModel>> getActive();
}

class CouponRemoteDataSourceImpl implements CouponRemoteDataSource {
  CouponRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<List<CouponModel>> getActive() async {
    final response = await _dio.get<List<dynamic>>('/api/coupons/active');
    final body = response.data ?? const [];
    return body
        .whereType<Map<String, dynamic>>()
        .map(CouponModel.fromJson)
        .toList(growable: false);
  }
}
