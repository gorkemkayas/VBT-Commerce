import 'package:freezed_annotation/freezed_annotation.dart';

import '../../domain/entities/user.dart';

part 'user_model.freezed.dart';
part 'user_model.g.dart';

@freezed
abstract class UserModel extends User with _$UserModel {
  const UserModel._({
    required super.id,
    required super.email,
    required super.accessToken,
    required super.refreshToken,
  });

  const factory UserModel({
    required String id,
    required String email,
    required String accessToken,
    required String refreshToken,
  }) = _UserModel;

  factory UserModel.fromJson(Map<String, dynamic> json) =>
      _$UserModelFromJson(json);

  /// Backend'in `AuthResponse` şekli: `{accessToken, accessTokenExpiresAt,
  /// refreshToken}` — kullanıcı id/email içermez, bu yüzden girişte
  /// kullanılan e-posta kimlik olarak kullanılıyor.
  factory UserModel.fromLoginResponse(Map<String, dynamic> json, String email) {
    final accessToken = json['accessToken'];
    if (accessToken is! String || accessToken.isEmpty) {
      throw const FormatException(
        'Giriş yanıtında erişim anahtarı bulunamadı.',
      );
    }
    final refreshToken = json['refreshToken'];
    if (refreshToken is! String || refreshToken.isEmpty) {
      throw const FormatException(
        'Giriş yanıtında yenileme anahtarı bulunamadı.',
      );
    }
    return UserModel(
      id: email,
      email: email,
      accessToken: accessToken,
      refreshToken: refreshToken,
    );
  }

  /// Backend'in `/api/auth/register` yanıtı da aynı `AuthResponse` şeklini
  /// kullanır, bu yüzden ayrıştırma mantığı `fromLoginResponse` ile aynıdır.
  factory UserModel.fromRegisterResponse(
    Map<String, dynamic> json,
    String email,
  ) => UserModel.fromLoginResponse(json, email);
}
