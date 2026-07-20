import 'dart:convert';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/services/storage_service.dart';
import '../models/user_model.dart';

abstract interface class AuthLocalDataSource {
  Future<void> saveUser(UserModel user);
  Future<UserModel?> getCachedUser();
  Future<void> clearUser();
}

class AuthLocalDataSourceImpl implements AuthLocalDataSource {
  AuthLocalDataSourceImpl(this._storage, this._secureStorage);
  final StorageService _storage;
  final SecureStorageService _secureStorage;

  @override
  Future<void> saveUser(UserModel user) async {
    await _storage.setString(
      StorageKeys.currentUser,
      jsonEncode({'id': user.id, 'email': user.email}),
    );
    await _secureStorage.setString(StorageKeys.accessToken, user.accessToken);
    await _secureStorage.setString(StorageKeys.refreshToken, user.refreshToken);
  }

  @override
  Future<UserModel?> getCachedUser() async {
    final rawUser = _storage.getString(StorageKeys.currentUser);
    if (rawUser == null) return null;
    final accessToken = await _secureStorage.getString(StorageKeys.accessToken);
    final refreshToken = await _secureStorage.getString(
      StorageKeys.refreshToken,
    );
    if (accessToken == null || refreshToken == null) return null;
    final json = jsonDecode(rawUser) as Map<String, dynamic>;
    return UserModel(
      id: json['id'] as String,
      email: json['email'] as String,
      accessToken: accessToken,
      refreshToken: refreshToken,
    );
  }

  @override
  Future<void> clearUser() async {
    await _storage.remove(StorageKeys.currentUser);
    await _secureStorage.remove(StorageKeys.accessToken);
    await _secureStorage.remove(StorageKeys.refreshToken);
  }
}
