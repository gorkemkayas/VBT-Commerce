import 'dart:convert';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/storage_service.dart';
import '../models/cart_item_snapshot.dart';

/// Sepetin gerçek kaynağı artık backend'dir; bu data source yalnızca
/// "Sepete Ekle" anında alınan ürün adı/görsel/fiyat anlık görüntüsünü
/// (snapshot), backend'in sepet kalemi kimliğine (`itemId`) göre yerelde
/// saklar.
abstract interface class LocalCartDataSource {
  Future<Map<String, CartItemSnapshot>> getSnapshots();
  Future<void> saveSnapshot(String itemId, CartItemSnapshot snapshot);
  Future<void> removeSnapshot(String itemId);
  Future<void> clearSnapshots();
}

class LocalCartDataSourceImpl implements LocalCartDataSource {
  LocalCartDataSourceImpl(this._storage);
  final StorageService _storage;

  @override
  Future<Map<String, CartItemSnapshot>> getSnapshots() async {
    final raw = _storage.getString(StorageKeys.cartItemSnapshots);
    if (raw == null || raw.isEmpty) return {};
    final decoded = jsonDecode(raw);
    if (decoded is! Map) throw const FormatException('Sepet biçimi geçersiz.');
    return decoded.map(
      (key, value) => MapEntry(
        key as String,
        CartItemSnapshot.fromJson(value as Map<String, dynamic>),
      ),
    );
  }

  @override
  Future<void> saveSnapshot(String itemId, CartItemSnapshot snapshot) async {
    final snapshots = await getSnapshots();
    snapshots[itemId] = snapshot;
    await _writeSnapshots(snapshots);
  }

  @override
  Future<void> removeSnapshot(String itemId) async {
    final snapshots = await getSnapshots();
    snapshots.remove(itemId);
    await _writeSnapshots(snapshots);
  }

  @override
  Future<void> clearSnapshots() async {
    await _storage.remove(StorageKeys.cartItemSnapshots);
  }

  Future<void> _writeSnapshots(Map<String, CartItemSnapshot> snapshots) async {
    final saved = await _storage.setString(
      StorageKeys.cartItemSnapshots,
      jsonEncode(snapshots.map((key, value) => MapEntry(key, value.toJson()))),
    );
    if (!saved) throw StateError('Sepet kaydedilemedi.');
  }
}
