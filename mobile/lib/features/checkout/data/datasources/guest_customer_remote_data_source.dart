import 'package:dio/dio.dart';

import '../models/guest_contact_model.dart';

abstract interface class GuestCustomerRemoteDataSource {
  Future<String> create({
    required String firstName,
    required String lastName,
    required String email,
    required String phoneNumber,
  });

  /// `GET /api/guest-customers/{guestCustomerId}` — daha önce oluşturulmuş
  /// misafir kaydını okur.
  Future<GuestContactModel> getById(String guestCustomerId);
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

  @override
  Future<GuestContactModel> getById(String guestCustomerId) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/guest-customers/$guestCustomerId',
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş misafir bilgisi alındı.');
    }
    return GuestContactModel.fromJson(body);
  }
}
