import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../data/datasources/order_remote_data_source.dart';
import '../../data/datasources/shipping_company_remote_data_source.dart';
import '../../data/repositories/checkout_repository_impl.dart';
import '../../domain/entities/order.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../../domain/usecases/complete_order_use_case.dart';

final orderRemoteDataSourceProvider = Provider<OrderRemoteDataSource>(
  (ref) => OrderRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final shippingCompanyRemoteDataSourceProvider =
    Provider<ShippingCompanyRemoteDataSource>(
      (ref) => ShippingCompanyRemoteDataSourceImpl(ref.watch(dioProvider)),
    );
final checkoutRepositoryProvider = Provider<CheckoutRepository>(
  (ref) => CheckoutRepositoryImpl(
    ref.watch(orderRemoteDataSourceProvider),
    ref.watch(shippingCompanyRemoteDataSourceProvider),
  ),
);
final completeOrderUseCaseProvider = Provider<CompleteOrderUseCase>(
  (ref) => CompleteOrderUseCase(
    ref.watch(checkoutRepositoryProvider),
    ref.watch(cartRepositoryProvider),
  ),
);

class CheckoutState {
  const CheckoutState({
    this.selectedAddressId,
    this.isSubmitting = false,
    this.order,
    this.failure,
  });

  /// Customer feature'ındaki `CustomerAddress.id` — sipariş oluşturma bu
  /// id'yi kullanacak (bkz. `CompleteOrderUseCase`).
  final String? selectedAddressId;
  final bool isSubmitting;
  final Order? order;
  final Failure? failure;

  CheckoutState copyWith({
    String? selectedAddressId,
    bool? isSubmitting,
    Order? order,
    Failure? failure,
    bool clearFailure = false,
  }) => CheckoutState(
    selectedAddressId: selectedAddressId ?? this.selectedAddressId,
    isSubmitting: isSubmitting ?? this.isSubmitting,
    order: order ?? this.order,
    failure: clearFailure ? null : (failure ?? this.failure),
  );
}

class CheckoutController extends Notifier<CheckoutState> {
  @override
  CheckoutState build() => const CheckoutState();

  void selectAddress(String addressId) {
    state = state.copyWith(selectedAddressId: addressId, clearFailure: true);
  }

  Future<void> completeOrder(List<CartItem> items) async {
    state = state.copyWith(isSubmitting: true, clearFailure: true);
    final result = await ref.read(completeOrderUseCaseProvider)(
      addressId: state.selectedAddressId,
      items: items,
    );
    state = switch (result) {
      Success<Order>(:final value) => state.copyWith(
        isSubmitting: false,
        order: value,
      ),
      ResultFailure<Order>(:final failure) => state.copyWith(
        isSubmitting: false,
        failure: failure,
      ),
    };
  }
}

final checkoutControllerProvider =
    NotifierProvider<CheckoutController, CheckoutState>(CheckoutController.new);
