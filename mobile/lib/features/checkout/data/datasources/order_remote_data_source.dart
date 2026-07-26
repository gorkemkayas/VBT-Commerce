import 'package:dio/dio.dart';

abstract interface class OrderRemoteDataSource {
  Future<String> placeMyOrder({
    required String addressId,
    String? billingAddressId,
    required String shippingCompanyId,
    required List<String> couponCodes,
    required String cardHolderName,
    required String cardNumber,
    required String cardExpireMonth,
    required String cardExpireYear,
    required String cardCvc,
    required String buyerIdentityNumber,
  });
  Future<String> placeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required List<String> couponCodes,
    required String recipientName,
    required String phoneNumber,
    required String country,
    required String city,
    required String district,
    required String postalCode,
    required String addressLine1,
    String? addressLine2,
    String? billingRecipientName,
    String? billingCountry,
    String? billingCity,
    String? billingDistrict,
    String? billingPostalCode,
    String? billingAddressLine1,
    String? billingAddressLine2,
    required String cardHolderName,
    required String cardNumber,
    required String cardExpireMonth,
    required String cardExpireYear,
    required String cardCvc,
    required String buyerIdentityNumber,
  });
}

/// `POST /api/orders/me`. Backend, kart bilgisini ve kupon listesini zorunlu
/// tutuyor. Kart alanları artık `PaymentCardForm` ile kullanıcıdan alınır
/// (bkz. `CheckoutPage`) ve olduğu gibi buraya iletilir — burada sabit bir
/// değer üretilmez.
///
/// ÖNEMLİ: Ödeme sahte değildir — backend `IyzicoGateway` üzerinden gerçekten
/// iyzico sandbox'a istek atar (bkz. `appsettings.Development.json` →
/// `Iyzico.BaseUrl`). Bu yüzden kart, iyzico'nun *kendi* sandbox test
/// kartlarından biri olmak zorundadır; `4111...` gibi genel test numaraları
/// "Payment declined" (402) ile reddedilir.
class OrderRemoteDataSourceImpl implements OrderRemoteDataSource {
  OrderRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<String> placeMyOrder({
    required String addressId,
    String? billingAddressId,
    required String shippingCompanyId,
    required List<String> couponCodes,
    required String cardHolderName,
    required String cardNumber,
    required String cardExpireMonth,
    required String cardExpireYear,
    required String cardCvc,
    required String buyerIdentityNumber,
  }) async {
    final response = await _dio.post<dynamic>(
      '/api/orders/me',
      data: {
        'addressId': addressId,
        'billingAddressId': billingAddressId,
        'shippingCompanyId': shippingCompanyId,
        'couponCodes': couponCodes,
        'cardHolderName': cardHolderName,
        'cardNumber': cardNumber,
        'cardExpireMonth': cardExpireMonth,
        'cardExpireYear': cardExpireYear,
        'cardCvc': cardCvc,
        'buyerIdentityNumber': buyerIdentityNumber,
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

  /// `POST /api/orders/guest`.
  @override
  Future<String> placeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required List<String> couponCodes,
    required String recipientName,
    required String phoneNumber,
    required String country,
    required String city,
    required String district,
    required String postalCode,
    required String addressLine1,
    String? addressLine2,
    String? billingRecipientName,
    String? billingCountry,
    String? billingCity,
    String? billingDistrict,
    String? billingPostalCode,
    String? billingAddressLine1,
    String? billingAddressLine2,
    required String cardHolderName,
    required String cardNumber,
    required String cardExpireMonth,
    required String cardExpireYear,
    required String cardCvc,
    required String buyerIdentityNumber,
  }) async {
    final response = await _dio.post<dynamic>(
      '/api/orders/guest',
      data: {
        'guestCustomerId': guestCustomerId,
        'anonymousId': anonymousId,
        'shippingCompanyId': shippingCompanyId,
        'couponCodes': couponCodes,
        'recipientName': recipientName,
        'phoneNumber': phoneNumber,
        'country': country,
        'city': city,
        'district': district,
        'postalCode': postalCode,
        'addressLine1': addressLine1,
        'addressLine2': addressLine2,
        'billingRecipientName': billingRecipientName,
        'billingCountry': billingCountry,
        'billingCity': billingCity,
        'billingDistrict': billingDistrict,
        'billingPostalCode': billingPostalCode,
        'billingAddressLine1': billingAddressLine1,
        'billingAddressLine2': billingAddressLine2,
        'cardHolderName': cardHolderName,
        'cardNumber': cardNumber,
        'cardExpireMonth': cardExpireMonth,
        'cardExpireYear': cardExpireYear,
        'cardCvc': cardCvc,
        'buyerIdentityNumber': buyerIdentityNumber,
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
