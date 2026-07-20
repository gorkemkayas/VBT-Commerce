import '../../../../core/utils/result.dart';
import '../entities/customer_address.dart';
import '../repositories/customer_repository.dart';

class AddCustomerAddressUseCase {
  const AddCustomerAddressUseCase(this._repository);
  final CustomerRepository _repository;

  Future<Result<String>> call(CustomerAddressInput input) =>
      _repository.addAddress(input);
}
