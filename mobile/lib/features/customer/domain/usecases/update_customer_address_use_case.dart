import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/customer_address.dart';
import '../repositories/customer_repository.dart';

class UpdateCustomerAddressUseCase {
  const UpdateCustomerAddressUseCase(this._repository);
  final CustomerRepository _repository;

  Future<Result<bool>> call(String addressId, CustomerAddressInput input) {
    if (addressId.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz adres kimliği.')),
      );
    }
    return _repository.updateAddress(addressId, input);
  }
}
