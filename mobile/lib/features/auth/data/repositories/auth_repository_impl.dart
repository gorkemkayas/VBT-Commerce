import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_local_data_source.dart';
import '../datasources/auth_remote_data_source.dart';

class AuthRepositoryImpl implements AuthRepository {
  AuthRepositoryImpl(this._remote, this._local);

  final AuthRemoteDataSource _remote;
  final AuthLocalDataSource _local;

  @override
  Future<Result<User>> login({
    required String email,
    required String password,
  }) async {
    try {
      final user = await _remote.login(email: email, password: password);
      await _local.saveUser(user);
      return Result.success(user);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Giriş sırasında beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<User>> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  }) async {
    try {
      final user = await _remote.register(
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
      );
      await _local.saveUser(user);
      return Result.success(user);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Kayıt sırasında beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<User?>> getCachedUser() async {
    try {
      return Result.success(await _local.getCachedUser());
    } catch (_) {
      return const Result.failure(CacheFailure('Oturum bilgisi okunamadı.'));
    }
  }

  @override
  Future<Result<bool>> logout() async {
    try {
      final cachedUser = await _local.getCachedUser();
      try {
        // Ek güvenlik: sunucudaki refresh token'ı iptal eder (bkz.
        // backend `LogoutCommandHandler.Revoke()`) — web'in yaptığı gibi.
        // Bu çağrı başarısız olsa bile (ağ hatası, timeout, backend hatası)
        // kullanıcı yine de çıkış yapabilmeli; bu yüzden hata burada
        // yutulur ve local temizleme her zaman devam eder.
        await _remote.logout(cachedUser?.refreshToken);
      } catch (_) {
        // Kasıtlı olarak yutuluyor — bkz. yukarıdaki not.
      }
      await _local.clearUser();
      return const Result.success(true);
    } catch (_) {
      return const Result.failure(
        CacheFailure('Çıkış yapılırken bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> forgotPassword(String email) async {
    try {
      await _remote.forgotPassword(email);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Şifre sıfırlama isteği gönderilirken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<bool>> resetPassword({
    required String token,
    required String newPassword,
  }) async {
    try {
      await _remote.resetPassword(token: token, newPassword: newPassword);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Şifre sıfırlanırken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }
}
