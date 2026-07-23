import 'package:dio/dio.dart';

abstract interface class GuestCustomerRemoteDataSource {
  Future<String> create({
    required String firstName,
    required String lastName,
    required String email,
    required String phoneNumber,
  });
}

/// `POST /api/guest-customers` (bkz. `GuestCustomersController`). Misafir
/// checkout'ta, sipariş oluşturmadan önce iletişim bilgilerinden bir misafir
/// müşteri kaydı açılır; dönen id fiyat hesaplama ve sipariş oluşturma
/// çağrılarında kullanılır.
class GuestCustomerRemoteDataSourceImpl implements GuestCustomerRemoteDataSource {
  GuestCustomerRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<String> create({
    required String firstName,
    required String lastName,
    required String email,
    required String phoneNumber,
  }) async {
    final response = await _dio.post<dynamic>(
      '/api/guest-customers',
      data: {
        'firstName': firstName,
        'lastName': lastName,
        'email': email,
        'phoneNumber': phoneNumber,
      },
    );
    final id = response.data;
    if (id is! String || id.isEmpty) {
      throw const FormatException(
        'Misafir müşteri oluşturulurken sunucudan geçersiz yanıt alındı.',
      );
    }
    return id;
  }
}
