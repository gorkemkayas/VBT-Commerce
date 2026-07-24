import 'package:dio/dio.dart';

import '../../../../core/services/anonymous_id_service.dart';
import '../models/user_model.dart';

abstract interface class AuthRemoteDataSource {
  Future<UserModel> login({required String email, required String password});
  Future<UserModel> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  });
  Future<void> forgotPassword(String email);
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  AuthRemoteDataSourceImpl(this._dio, this._anonymousIdService);
  final Dio _dio;
  final AnonymousIdService _anonymousIdService;

  @override
  Future<UserModel> login({
    required String email,
    required String password,
  }) async {
    final anonymousId = await _anonymousIdService.getOrCreateAnonymousId();
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/auth/login',
      data: {
        'email': email,
        'password': password,
        'platform': 'Mobile',
        'anonymousId': anonymousId,
      },
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş yanıt alındı.');
    }
    return UserModel.fromLoginResponse(body, email);
  }

  @override
  Future<UserModel> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  }) async {
    final anonymousId = await _anonymousIdService.getOrCreateAnonymousId();
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/auth/register',
      data: {
        'email': email,
        'password': password,
        'firstName': firstName,
        'lastName': lastName,
        'platform': 'Mobile',
        'anonymousId': anonymousId,
      },
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş yanıt alındı.');
    }
    return UserModel.fromRegisterResponse(body, email);
  }

  /// `POST /api/auth/forgot-password`. Backend, e-postanın kayıtlı olup
  /// olmadığından bağımsız olarak her zaman `204 No Content` döner (bkz.
  /// `ForgotPasswordCommandHandler` — enumeration'ı önlemek için bilinçli
  /// tasarım); eşleşen bir hesap varsa arka planda sıfırlama e-postası
  /// gönderilir.
  @override
  Future<void> forgotPassword(String email) async {
    await _dio.post<void>('/api/auth/forgot-password', data: {'email': email});
  }
}
