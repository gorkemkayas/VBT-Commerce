import '../../../../core/utils/result.dart';
import '../repositories/customer_repository.dart';

class UpdateCustomerProfileUseCase {
  const UpdateCustomerProfileUseCase(this._repository);
  final CustomerRepository _repository;

  Future<Result<bool>> call({String? phoneNumber, String? dateOfBirth}) =>
      _repository.updateProfile(
        phoneNumber: phoneNumber,
        dateOfBirth: dateOfBirth,
      );
}
