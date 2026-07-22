import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../data/datasources/order_remote_data_source.dart';
import '../../data/datasources/pricing_remote_data_source.dart';
import '../../data/datasources/shipping_company_remote_data_source.dart';
import '../../data/repositories/checkout_repository_impl.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/price_calculation.dart';
import '../../domain/entities/shipping_company.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../../domain/usecases/calculate_price_use_case.dart';
import '../../domain/usecases/complete_order_use_case.dart';
import '../../domain/usecases/get_shipping_companies_use_case.dart';

final orderRemoteDataSourceProvider = Provider<OrderRemoteDataSource>(
  (ref) => OrderRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final shippingCompanyRemoteDataSourceProvider =
    Provider<ShippingCompanyRemoteDataSource>(
      (ref) => ShippingCompanyRemoteDataSourceImpl(ref.watch(dioProvider)),
    );
final pricingRemoteDataSourceProvider = Provider<PricingRemoteDataSource>(
  (ref) => PricingRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final checkoutRepositoryProvider = Provider<CheckoutRepository>(
  (ref) => CheckoutRepositoryImpl(
    ref.watch(orderRemoteDataSourceProvider),
    ref.watch(shippingCompanyRemoteDataSourceProvider),
    ref.watch(pricingRemoteDataSourceProvider),
  ),
);
final completeOrderUseCaseProvider = Provider<CompleteOrderUseCase>(
  (ref) => CompleteOrderUseCase(
    ref.watch(checkoutRepositoryProvider),
    ref.watch(cartRepositoryProvider),
  ),
);
final calculatePriceUseCaseProvider = Provider<CalculatePriceUseCase>(
  (ref) => CalculatePriceUseCase(ref.watch(checkoutRepositoryProvider)),
);
final getShippingCompaniesUseCaseProvider =
    Provider<GetShippingCompaniesUseCase>(
      (ref) => GetShippingCompaniesUseCase(ref.watch(checkoutRepositoryProvider)),
    );

/// Checkout açıldığında ve sepet her değiştiğinde (`cartControllerProvider`
/// izlendiği için) backend'in gerçek fiyat hesaplamasını yeniden çeker.
final priceCalculationProvider =
    FutureProvider.autoDispose<Result<PriceCalculation>>((ref) {
      final items = ref.watch(
        cartControllerProvider.select((state) => state.items),
      );
      return ref.watch(calculatePriceUseCaseProvider)(items);
    });

/// Aktif kargo firmalarının tamamını çeker; seçim UI'ı bunu izler.
final shippingCompaniesProvider =
    FutureProvider.autoDispose<Result<List<ShippingCompany>>>((ref) {
      return ref.watch(getShippingCompaniesUseCaseProvider)();
    });

/// Seçili kargo firmasının ücreti — `selectedShippingCompanyId` ya da liste
/// her değiştiğinde otomatik yeniden hesaplanır (bkz. `PaymentSummaryView`).
/// Henüz seçim yoksa veya liste yüklenmediyse `null` döner.
final selectedShippingFeeProvider = Provider.autoDispose<double?>((ref) {
  final selectedId = ref.watch(
    checkoutControllerProvider.select((state) => state.selectedShippingCompanyId),
  );
  if (selectedId == null) return null;
  final result = ref.watch(shippingCompaniesProvider).value;
  final companies = switch (result) {
    Success<List<ShippingCompany>>(:final value) => value,
    ResultFailure<List<ShippingCompany>>() => null,
    null => null,
  };
  if (companies == null) return null;
  for (final company in companies) {
    if (company.id == selectedId) return company.fee;
  }
  return null;
});

class CheckoutState {
  const CheckoutState({
    this.selectedAddressId,
    this.selectedShippingCompanyId,
    this.isSubmitting = false,
    this.order,
    this.failure,
  });

  /// Customer feature'ındaki `CustomerAddress.id` — sipariş oluşturma bu
  /// id'yi kullanacak (bkz. `CompleteOrderUseCase`).
  final String? selectedAddressId;

  /// Seçili kargo firmasının id'si — Checkout açıldığında ilk firma
  /// otomatik seçilir (bkz. `CheckoutController.build`), kullanıcı farklı
  /// bir firma seçerse güncellenir.
  final String? selectedShippingCompanyId;
  final bool isSubmitting;
  final Order? order;
  final Failure? failure;

  CheckoutState copyWith({
    String? selectedAddressId,
    String? selectedShippingCompanyId,
    bool? isSubmitting,
    Order? order,
    Failure? failure,
    bool clearFailure = false,
  }) => CheckoutState(
    selectedAddressId: selectedAddressId ?? this.selectedAddressId,
    selectedShippingCompanyId:
        selectedShippingCompanyId ?? this.selectedShippingCompanyId,
    isSubmitting: isSubmitting ?? this.isSubmitting,
    order: order ?? this.order,
    failure: clearFailure ? null : (failure ?? this.failure),
  );
}

class CheckoutController extends Notifier<CheckoutState> {
  @override
  CheckoutState build() {
    // Kargo firmaları ilk yüklendiğinde, kullanıcı henüz bir seçim
    // yapmadıysa listedeki ilk firma varsayılan olarak seçilir.
    ref.listen(shippingCompaniesProvider, (previous, next) {
      final result = next.value;
      if (result == null) return;
      final companies = switch (result) {
        Success<List<ShippingCompany>>(:final value) => value,
        ResultFailure<List<ShippingCompany>>() => null,
      };
      if (companies != null &&
          companies.isNotEmpty &&
          state.selectedShippingCompanyId == null) {
        state = state.copyWith(selectedShippingCompanyId: companies.first.id);
      }
    });
    return const CheckoutState();
  }

  void selectAddress(String addressId) {
    state = state.copyWith(selectedAddressId: addressId, clearFailure: true);
  }

  void selectShippingCompany(String shippingCompanyId) {
    state = state.copyWith(
      selectedShippingCompanyId: shippingCompanyId,
      clearFailure: true,
    );
  }

  Future<void> completeOrder(List<CartItem> items) async {
    state = state.copyWith(isSubmitting: true, clearFailure: true);
    final result = await ref.read(completeOrderUseCaseProvider)(
      addressId: state.selectedAddressId,
      shippingCompanyId: state.selectedShippingCompanyId,
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
