import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/customer_remote_data_source.dart';
import '../../data/repositories/customer_repository_impl.dart';
import '../../domain/entities/customer.dart';
import '../../domain/entities/customer_address.dart';
import '../../domain/repositories/customer_repository.dart';
import '../../domain/usecases/add_customer_address_use_case.dart';
import '../../domain/usecases/delete_customer_address_use_case.dart';
import '../../domain/usecases/get_current_customer_use_case.dart';
import '../../domain/usecases/set_default_customer_address_use_case.dart';
import '../../domain/usecases/update_customer_address_use_case.dart';

final customerRemoteDataSourceProvider = Provider<CustomerRemoteDataSource>(
  (ref) => CustomerRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final customerRepositoryProvider = Provider<CustomerRepository>(
  (ref) => CustomerRepositoryImpl(ref.watch(customerRemoteDataSourceProvider)),
);
final getCurrentCustomerUseCaseProvider = Provider<GetCurrentCustomerUseCase>(
  (ref) => GetCurrentCustomerUseCase(ref.watch(customerRepositoryProvider)),
);
final addCustomerAddressUseCaseProvider = Provider<AddCustomerAddressUseCase>(
  (ref) => AddCustomerAddressUseCase(ref.watch(customerRepositoryProvider)),
);
final updateCustomerAddressUseCaseProvider =
    Provider<UpdateCustomerAddressUseCase>(
      (ref) =>
          UpdateCustomerAddressUseCase(ref.watch(customerRepositoryProvider)),
    );
final deleteCustomerAddressUseCaseProvider =
    Provider<DeleteCustomerAddressUseCase>(
      (ref) =>
          DeleteCustomerAddressUseCase(ref.watch(customerRepositoryProvider)),
    );
final setDefaultCustomerAddressUseCaseProvider =
    Provider<SetDefaultCustomerAddressUseCase>(
      (ref) => SetDefaultCustomerAddressUseCase(
        ref.watch(customerRepositoryProvider),
      ),
    );

final currentCustomerProvider = FutureProvider.autoDispose<Result<Customer>>(
  (ref) => ref.watch(getCurrentCustomerUseCaseProvider)(),
);

class AddressesState {
  const AddressesState({
    this.isLoading = false,
    this.addresses = const [],
    this.failure,
  });
  final bool isLoading;
  final List<CustomerAddress> addresses;
  final Failure? failure;
}

class AddressesController extends Notifier<AddressesState> {
  @override
  AddressesState build() {
    Future.microtask(loadAddresses);
    return const AddressesState(isLoading: true);
  }

  Future<void> loadAddresses() async {
    state = AddressesState(isLoading: true, addresses: state.addresses);
    final result = await ref.read(getCurrentCustomerUseCaseProvider)();
    state = switch (result) {
      Success<Customer>(:final value) => AddressesState(
        addresses: value.addresses,
      ),
      ResultFailure<Customer>(:final failure) => AddressesState(
        addresses: state.addresses,
        failure: failure,
      ),
    };
  }

  Future<Result<String>> addAddress(CustomerAddressInput input) async {
    final result = await ref.read(addCustomerAddressUseCaseProvider)(input);
    if (result is Success<String>) await loadAddresses();
    return result;
  }

  Future<Result<bool>> updateAddress(
    String addressId,
    CustomerAddressInput input,
  ) async {
    final result = await ref.read(updateCustomerAddressUseCaseProvider)(
      addressId,
      input,
    );
    if (result is Success<bool>) await loadAddresses();
    return result;
  }

  Future<Result<bool>> deleteAddress(String addressId) async {
    final result = await ref.read(deleteCustomerAddressUseCaseProvider)(
      addressId,
    );
    if (result is Success<bool>) await loadAddresses();
    return result;
  }

  Future<Result<bool>> setDefaultAddress(String addressId) async {
    final result = await ref.read(setDefaultCustomerAddressUseCaseProvider)(
      addressId,
    );
    if (result is Success<bool>) await loadAddresses();
    return result;
  }
}

final addressesControllerProvider =
    NotifierProvider<AddressesController, AddressesState>(
      AddressesController.new,
    );
