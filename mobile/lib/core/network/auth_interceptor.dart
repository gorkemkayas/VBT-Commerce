// Alan adları private (_secureStorage vb.) olduğu için dışarıdan çağrılabilir
// isimlerle (secureStorage vb.) elle atanıyor — initializing formal kullanılamaz.
// ignore_for_file: prefer_initializing_formals

import 'package:dio/dio.dart';
import 'package:flutter/widgets.dart';
import 'package:go_router/go_router.dart';

import '../constants/app_constants.dart';
import '../constants/route_paths.dart';
import '../constants/storage_keys.dart';
import '../services/secure_storage_service.dart';
import '../services/storage_service.dart';

/// Her isteğe `Authorization: Bearer <accessToken>` ekler. 401 alınırsa
/// `/api/auth/refresh` ile yeni token alıp orijinal isteği bir kez tekrar
/// dener; yenileme de başarısız olursa oturumu temizleyip login ekranına
/// yönlendirir.
class AuthInterceptor extends Interceptor {
  AuthInterceptor({
    required SecureStorageService secureStorage,
    required StorageService storage,
    required GlobalKey<NavigatorState> navigatorKey,
  }) : _secureStorage = secureStorage,
       _storage = storage,
       _navigatorKey = navigatorKey,
       _refreshDio = Dio(BaseOptions(baseUrl: AppConstants.apiBaseUrl));

  final SecureStorageService _secureStorage;
  final StorageService _storage;
  final GlobalKey<NavigatorState> _navigatorKey;
  final Dio _refreshDio;

  static const _authPaths = [
    '/api/auth/login',
    '/api/auth/register',
    '/api/auth/refresh',
  ];

  bool _isAuthPath(String path) =>
      _authPaths.any((authPath) => path.contains(authPath));

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    if (!_isAuthPath(options.path)) {
      final accessToken = await _secureStorage.getString(
        StorageKeys.accessToken,
      );
      if (accessToken != null) {
        options.headers['Authorization'] = 'Bearer $accessToken';
      }
    }
    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final requestOptions = err.requestOptions;
    final isUnauthorized = err.response?.statusCode == 401;
    final alreadyRetried = requestOptions.extra['retried'] == true;

    if (!isUnauthorized || alreadyRetried || _isAuthPath(requestOptions.path)) {
      return handler.next(err);
    }

    final refreshToken = await _secureStorage.getString(
      StorageKeys.refreshToken,
    );
    if (refreshToken == null) {
      await _forceLogout();
      return handler.next(err);
    }

    try {
      final response = await _refreshDio.post<Map<String, dynamic>>(
        '/api/auth/refresh',
        data: {'refreshToken': refreshToken},
      );
      final body = response.data;
      final newAccessToken = body?['accessToken'];
      final newRefreshToken = body?['refreshToken'];
      if (newAccessToken is! String || newRefreshToken is! String) {
        throw const FormatException('Yenileme yanıtı geçersiz.');
      }

      await _secureStorage.setString(StorageKeys.accessToken, newAccessToken);
      await _secureStorage.setString(StorageKeys.refreshToken, newRefreshToken);

      requestOptions.headers['Authorization'] = 'Bearer $newAccessToken';
      requestOptions.extra['retried'] = true;
      final retryResponse = await _refreshDio.fetch(requestOptions);
      return handler.resolve(retryResponse);
    } catch (_) {
      await _forceLogout();
      return handler.next(err);
    }
  }

  Future<void> _forceLogout() async {
    await _storage.remove(StorageKeys.currentUser);
    await _secureStorage.remove(StorageKeys.accessToken);
    await _secureStorage.remove(StorageKeys.refreshToken);
    final context = _navigatorKey.currentContext;
    if (context != null && context.mounted) {
      context.go(RoutePaths.login);
    }
  }
}
