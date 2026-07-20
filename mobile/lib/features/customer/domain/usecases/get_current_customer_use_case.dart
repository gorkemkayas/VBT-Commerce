import '../../../../core/utils/result.dart';
import '../entities/customer.dart';
import '../repositories/customer_repository.dart';

class GetCurrentCustomerUseCase {
  const GetCurrentCustomerUseCase(this._repository);
  final CustomerRepository _repository;

  Future<Result<Customer>> call() => _repository.getCurrentCustomer();
}
