import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

final secureStorageServiceProvider = Provider<SecureStorageService>(
  (ref) => SecureStorageService(const FlutterSecureStorage()),
);

class SecureStorageService {
  SecureStorageService(this._storage);
  final FlutterSecureStorage _storage;

  Future<void> setString(String key, String value) =>
      _storage.write(key: key, value: value);
  Future<String?> getString(String key) => _storage.read(key: key);
  Future<void> remove(String key) => _storage.delete(key: key);
}
