import 'package:dio/dio.dart';

abstract interface class OrderRemoteDataSource {
  Future<String> placeMyOrder({
    required String addressId,
    required String shippingCompanyId,
  });
  Future<String> placeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required String recipientName,
    required String phoneNumber,
    required String country,
    required String city,
    required String district,
    required String postalCode,
    required String addressLine1,
    String? addressLine2,
  });
}

/// `POST /api/orders/me`. Backend, kart bilgisini ve kupon listesini zorunlu
/// tutuyor. Ödeme formu ve kupon girişi bu görevin kapsamı dışında olduğu için
/// bu alanlar sabit değerlerle gönderilir; ayrı bir görevde gerçek form/seçimle
/// değiştirilecek.
///
/// ÖNEMLİ: Ödeme sahte değildir — backend `IyzicoGateway` üzerinden gerçekten
/// iyzico sandbox'a istek atar (bkz. `appsettings.Development.json` →
/// `Iyzico.BaseUrl`). Bu yüzden kart, iyzico'nun *kendi* sandbox test
/// kartlarından biri olmak zorundadır; `4111...` gibi genel test numaraları
/// "Payment declined" (402) ile reddedilir. Aşağıdaki kart/kimlik numarası
/// iyzico'nun dokümante ettiği sandbox test değerleridir.
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
        // iyzico sandbox test kartı (Halkbank, Master Card - Debit).
        'cardHolderName': 'John Doe',
        'cardNumber': '5528790000000008',
        'cardExpireMonth': '12',
        'cardExpireYear': '2030',
        'cardCvc': '123',
        'buyerIdentityNumber': '74300864791',
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

  /// `POST /api/orders/guest`. Aynı iyzico sandbox test kartı `placeMyOrder`
  /// ile paylaşılır — bkz. yukarıdaki ÖNEMLİ notu.
  @override
  Future<String> placeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required String recipientName,
    required String phoneNumber,
    required String country,
    required String city,
    required String district,
    required String postalCode,
    required String addressLine1,
    String? addressLine2,
  }) async {
    final response = await _dio.post<dynamic>(
      '/api/orders/guest',
      data: {
        'guestCustomerId': guestCustomerId,
        'anonymousId': anonymousId,
        'shippingCompanyId': shippingCompanyId,
        'couponCodes': <String>[],
        'recipientName': recipientName,
        'phoneNumber': phoneNumber,
        'country': country,
        'city': city,
        'district': district,
        'postalCode': postalCode,
        'addressLine1': addressLine1,
        'addressLine2': addressLine2,
        // iyzico sandbox test kartı (Halkbank, Master Card - Debit).
        'cardHolderName': 'John Doe',
        'cardNumber': '5528790000000008',
        'cardExpireMonth': '12',
        'cardExpireYear': '2030',
        'cardCvc': '123',
        'buyerIdentityNumber': '74300864791',
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
