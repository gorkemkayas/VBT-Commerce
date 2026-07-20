import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../constants/storage_keys.dart';
import 'storage_service.dart';

final anonymousIdServiceProvider = Provider<AnonymousIdService>(
  (ref) => AnonymousIdService(ref.watch(storageServiceProvider)),
);

/// Misafir sepeti için cihazda saklanan kimlik. Backend değil, uygulama
/// üretir; hassas olmadığı için düz local storage'da (SharedPreferences)
/// tutulur.
class AnonymousIdService {
  AnonymousIdService(this._storage);
  final StorageService _storage;

  Future<String> getOrCreateAnonymousId() async {
    final existing = _storage.getString(StorageKeys.anonymousId);
    if (existing != null && existing.isNotEmpty) return existing;
    final id = const Uuid().v4();
    await _storage.setString(StorageKeys.anonymousId, id);
    return id;
  }
}
