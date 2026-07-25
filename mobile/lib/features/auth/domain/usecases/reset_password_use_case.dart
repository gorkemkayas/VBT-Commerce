import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../repositories/auth_repository.dart';

class ResetPasswordUseCase {
  const ResetPasswordUseCase(this._repository);
  final AuthRepository _repository;

  Future<Result<bool>> call({
    required String token,
    required String newPassword,
  }) {
    final trimmedToken = token.trim();
    if (trimmedToken.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Sıfırlama kodu zorunludur.')),
      );
    }
    if (newPassword.length < 8) {
      return Future.value(
        const Result.failure(
          ValidationFailure('Şifre en az 8 karakter olmalı.'),
        ),
      );
    }
    return _repository.resetPassword(
      token: trimmedToken,
      newPassword: newPassword,
    );
  }
}
