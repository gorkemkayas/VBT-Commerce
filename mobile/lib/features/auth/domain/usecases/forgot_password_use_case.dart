import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../repositories/auth_repository.dart';

final _emailRegExp = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$');

class ForgotPasswordUseCase {
  const ForgotPasswordUseCase(this._repository);
  final AuthRepository _repository;

  Future<Result<bool>> call(String email) {
    final normalizedEmail = email.trim();
    if (normalizedEmail.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('E-posta adresi zorunludur.')),
      );
    }
    if (!_emailRegExp.hasMatch(normalizedEmail)) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçerli bir e-posta girin.')),
      );
    }
    return _repository.forgotPassword(normalizedEmail);
  }
}
