import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/user.dart';
import '../repositories/auth_repository.dart';

class LoginUseCase {
  const LoginUseCase(this._repository);
  final AuthRepository _repository;

  Future<Result<User>> call({required String email, required String password}) {
    final normalizedEmail = email.trim();
    if (normalizedEmail.isEmpty || !normalizedEmail.contains('@')) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçerli bir e-posta girin.')),
      );
    }
    if (password.length < 6) {
      return Future.value(
        const Result.failure(
          ValidationFailure('Şifre en az 6 karakter olmalıdır.'),
        ),
      );
    }
    return _repository.login(email: normalizedEmail, password: password);
  }
}
