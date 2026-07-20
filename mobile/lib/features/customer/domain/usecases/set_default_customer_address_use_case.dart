import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../repositories/customer_repository.dart';

class SetDefaultCustomerAddressUseCase {
  const SetDefaultCustomerAddressUseCase(this._repository);
  final CustomerRepository _repository;

  Future<Result<bool>> call(String addressId) {
    if (addressId.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz adres kimliği.')),
      );
    }
    return _repository.setDefaultAddress(addressId);
  }
}
