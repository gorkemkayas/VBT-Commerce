import '../../../../core/utils/result.dart';
import '../entities/user.dart';

abstract interface class AuthRepository {
  Future<Result<User>> login({required String email, required String password});
  Future<Result<User>> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  });
  Future<Result<User?>> getCachedUser();
  Future<Result<bool>> logout();
}
