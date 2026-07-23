import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:commerce_mobile/app.dart';
import 'package:commerce_mobile/core/services/secure_storage_service.dart';
import 'package:commerce_mobile/core/services/storage_service.dart';

/// Testte gerçek `flutter_secure_storage` platform kanalını çağırmamak için
/// bellek içi bir sahte implementasyon — aksi halde `AuthInterceptor`'ın her
/// istekte yaptığı okuma, mock'lanmamış plugin kanalında zaman aşımına uğrar.
class _FakeSecureStorageService extends SecureStorageService {
  _FakeSecureStorageService() : super(const FlutterSecureStorage());
  final Map<String, String> _values = {};

  @override
  Future<void> setString(String key, String value) async {
    _values[key] = value;
  }

  @override
  Future<String?> getString(String key) async => _values[key];

  @override
  Future<void> remove(String key) async {
    _values.remove(key);
  }
}

void main() {
  testWidgets('ana sayfa ekranı açılır', (tester) async {
    SharedPreferences.setMockInitialValues({});
    final preferences = await SharedPreferences.getInstance();
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          sharedPreferencesProvider.overrideWithValue(preferences),
          secureStorageServiceProvider.overrideWithValue(
            _FakeSecureStorageService(),
          ),
        ],
        child: const SneakerStoreApp(),
      ),
    );
    await tester.pumpAndSettle();
    expect(find.text('Ürünler'), findsOneWidget);
  });
}
