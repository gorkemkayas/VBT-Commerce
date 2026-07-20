import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../constants/app_constants.dart';
import '../router/navigation_service.dart';
import '../services/secure_storage_service.dart';
import '../services/storage_service.dart';
import 'auth_interceptor.dart';

final dioProvider = Provider<Dio>((ref) {
  final dio = Dio(
    BaseOptions(
      baseUrl: AppConstants.apiBaseUrl,
      connectTimeout: AppConstants.connectTimeout,
      receiveTimeout: AppConstants.receiveTimeout,
      headers: const {'Content-Type': 'application/json'},
    ),
  );
  dio.interceptors.add(
    AuthInterceptor(
      secureStorage: ref.watch(secureStorageServiceProvider),
      storage: ref.watch(storageServiceProvider),
      navigatorKey: rootNavigatorKey,
    ),
  );
  return dio;
});
