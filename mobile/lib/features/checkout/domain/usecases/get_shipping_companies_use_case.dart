import '../../../../core/utils/result.dart';
import '../entities/shipping_company.dart';
import '../repositories/checkout_repository.dart';

class GetShippingCompaniesUseCase {
  const GetShippingCompaniesUseCase(this._repository);
  final CheckoutRepository _repository;

  Future<Result<List<ShippingCompany>>> call() =>
      _repository.getShippingCompanies();
}
