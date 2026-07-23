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

/// Backend hataları RFC 7807 `ProblemDetails` olarak döner (bkz.
/// `GlobalExceptionHandler`): `{status, title, detail}`. Asıl sebep `detail`
/// alanındadır — bu yüzden önce o okunur. `error`/`message` alanları başka
/// uçlardan gelebilecek biçimler için korunur; hiçbiri yoksa `title`, o da
/// yoksa genel mesaj kullanılır.
String _responseMessage(Object? data) {
  if (data case {'detail': final String message} when message.isNotEmpty) {
    return message;
  }
  if (data case {'error': final String message} when message.isNotEmpty) {
    return message;
  }
  if (data case {'message': final String message} when message.isNotEmpty) {
    return message;
  }
  if (data case {'title': final String message} when message.isNotEmpty) {
    return message;
  }
  return 'Sunucu isteği işleyemedi.';
}
