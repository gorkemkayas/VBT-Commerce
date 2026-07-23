import '../../../../core/utils/result.dart';
import '../entities/customer.dart';
import '../entities/customer_address.dart';

abstract interface class CustomerRepository {
  Future<Result<Customer>> getCurrentCustomer();
  Future<Result<bool>> updateProfile({
    String? phoneNumber,
    String? dateOfBirth,
  });
  Future<Result<String>> addAddress(CustomerAddressInput input);
  Future<Result<bool>> updateAddress(
    String addressId,
    CustomerAddressInput input,
  );
  Future<Result<bool>> deleteAddress(String addressId);
  Future<Result<bool>> setDefaultAddress(String addressId);
}
