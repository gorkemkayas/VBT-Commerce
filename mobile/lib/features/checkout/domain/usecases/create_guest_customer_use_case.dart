import '../../../../core/utils/result.dart';
import '../entities/guest_checkout_info.dart';
import '../repositories/checkout_repository.dart';

class CreateGuestCustomerUseCase {
  const CreateGuestCustomerUseCase(this._repository);
  final CheckoutRepository _repository;

  Future<Result<String>> call(GuestCheckoutInfo info) =>
      _repository.createGuestCustomer(
        firstName: info.firstName,
        lastName: info.lastName,
        email: info.email,
        phoneNumber: info.phoneNumber,
      );
}
