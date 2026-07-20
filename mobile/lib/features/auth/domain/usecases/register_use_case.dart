import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/user.dart';
import '../repositories/auth_repository.dart';

class RegisterUseCase {
  const RegisterUseCase(this._repository);
  final AuthRepository _repository;

  Future<Result<User>> call({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  }) {
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
    if (firstName.trim().isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Ad zorunludur.')),
      );
    }
    if (lastName.trim().isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Soyad zorunludur.')),
      );
    }
    return _repository.register(
      email: normalizedEmail,
      password: password,
      firstName: firstName.trim(),
      lastName: lastName.trim(),
    );
  }
}
