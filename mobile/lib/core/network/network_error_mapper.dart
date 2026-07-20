import 'package:dio/dio.dart';
import '../errors/failure.dart';

Failure mapDioException(DioException error) => switch (error.type) {
  DioExceptionType.connectionTimeout ||
  DioExceptionType.sendTimeout ||
  DioExceptionType.receiveTimeout ||
  DioExceptionType.transformTimeout ||
  DioExceptionType.connectionError => const NetworkFailure(
    'İnternet bağlantınızı kontrol edip tekrar deneyin.',
  ),
  DioExceptionType.badResponse => ServerFailure(
    _responseMessage(error.response?.data),
    statusCode: error.response?.statusCode,
  ),
  DioExceptionType.cancel => const NetworkFailure('İstek iptal edildi.'),
  DioExceptionType.badCertificate || DioExceptionType.unknown =>
    const UnknownFailure('Beklenmeyen bir hata oluştu.'),
};

String _responseMessage(Object? data) {
  if (data case {'error': final String message}) return message;
  if (data case {'message': final String message}) return message;
  return 'Sunucu isteği işleyemedi.';
}
