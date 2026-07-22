import 'package:dio/dio.dart';

abstract interface class OrderRemoteDataSource {
  Future<String> placeMyOrder({
    required String addressId,
    required String shippingCompanyId,
  });
}

/// `POST /api/orders/me`. Backend, kart bilgisini ve kupon listesini zorunlu
/// tutuyor (gerçek bir ödeme ağ geçidi yok, sadece format doğrulaması var) —
/// ödeme formu ve kupon girişi bu görevin kapsamı dışında olduğu için bu
/// alanlar için sabit placeholder değerler gönderilir. Bunlar ayrı bir
/// görevde gerçek form/seçimle değiştirilecek.
class OrderRemoteDataSourceImpl implements OrderRemoteDataSource {
  OrderRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<String> placeMyOrder({
    required String addressId,
    required String shippingCompanyId,
  }) async {
    final response = await _dio.post<dynamic>(
      '/api/orders/me',
      data: {
        'addressId': addressId,
        'shippingCompanyId': shippingCompanyId,
        'couponCodes': <String>[],
        'cardHolderName': 'Test Kullanıcı',
        'cardNumber': '4111111111111111',
        'cardExpireMonth': '12',
        'cardExpireYear': '2030',
        'cardCvc': '123',
        'buyerIdentityNumber': '11111111111',
      },
    );
    final id = response.data;
    if (id is! String || id.isEmpty) {
      throw const FormatException(
        'Sipariş oluşturulurken sunucudan geçersiz yanıt alındı.',
      );
    }
    return id;
  }
}
