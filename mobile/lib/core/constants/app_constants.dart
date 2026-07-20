abstract final class AppConstants {
  /// CI/CD veya çalıştırma sırasında `--dart-define=API_BASE_URL=...` ile değiştirin.
  static const apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'https://intern-api.kayas.dev',
  );
  static const connectTimeout = Duration(seconds: 15);
  static const receiveTimeout = Duration(seconds: 15);

  /// Product ve Auth aynı backend'i kullanıyor; ayrı ortamlar gerekirse
  /// `--dart-define=PRODUCT_API_BASE_URL=...` ile override edin.
  static const productApiBaseUrl = String.fromEnvironment(
    'PRODUCT_API_BASE_URL',
    defaultValue: 'https://intern-api.kayas.dev',
  );
}
