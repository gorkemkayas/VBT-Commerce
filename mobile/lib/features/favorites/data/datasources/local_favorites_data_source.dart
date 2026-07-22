import 'dart:convert';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/storage_service.dart';
import '../models/favorite_item_model.dart';

/// Favoriler backend'de tutulmadığından tek gerçek kaynak yereldir. Liste
/// ekleme sırasını korumak için JSON dizisi olarak saklanır.
abstract interface class LocalFavoritesDataSource {
  Future<List<FavoriteItemModel>> getFavorites();
  Future<void> saveFavorites(List<FavoriteItemModel> favorites);
}

class LocalFavoritesDataSourceImpl implements LocalFavoritesDataSource {
  LocalFavoritesDataSourceImpl(this._storage);
  final StorageService _storage;

  @override
  Future<List<FavoriteItemModel>> getFavorites() async {
    final raw = _storage.getString(StorageKeys.favoriteItems);
    if (raw == null || raw.isEmpty) return [];
    final decoded = jsonDecode(raw);
    if (decoded is! List) {
      throw const FormatException('Favori listesi biçimi geçersiz.');
    }
    return decoded
        .map(
          (item) => FavoriteItemModel.fromJson(item as Map<String, dynamic>),
        )
        .toList(growable: false);
  }

  @override
  Future<void> saveFavorites(List<FavoriteItemModel> favorites) async {
    final saved = await _storage.setString(
      StorageKeys.favoriteItems,
      jsonEncode(favorites.map((item) => item.toJson()).toList()),
    );
    if (!saved) throw StateError('Favoriler kaydedilemedi.');
  }
}
