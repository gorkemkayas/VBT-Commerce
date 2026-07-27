import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/guest_contact.dart';
import '../repositories/checkout_repository.dart';

/// Cihazda saklanan misafir müşteri id'sinden iletişim bilgilerini getirir —
/// misafir checkout formunun önceden doldurulması için (bkz.
/// `savedGuestContactProvider`).
class GetGuestCustomerUseCase {
  const GetGuestCustomerUseCase(this._repository);
  final CheckoutRepository _repository;

  Future<Result<GuestContact>> call(String guestCustomerId) {
    if (guestCustomerId.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz misafir kaydı.')),
      );
    }
    return _repository.getGuestCustomer(guestCustomerId);
  }
}
