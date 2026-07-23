import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/services/anonymous_id_service.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../data/datasources/guest_customer_remote_data_source.dart';
import '../../data/datasources/order_remote_data_source.dart';
import '../../data/datasources/pricing_remote_data_source.dart';
import '../../data/datasources/shipping_company_remote_data_source.dart';
import '../../data/repositories/checkout_repository_impl.dart';
import '../../domain/entities/guest_checkout_info.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/price_calculation.dart';
import '../../domain/entities/shipping_company.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../../domain/usecases/calculate_price_guest_use_case.dart';
import '../../domain/usecases/calculate_price_use_case.dart';
import '../../domain/usecases/complete_guest_order_use_case.dart';
import '../../domain/usecases/complete_order_use_case.dart';
import '../../domain/usecases/create_guest_customer_use_case.dart';
import '../../domain/usecases/get_shipping_companies_use_case.dart';

const _unset = Object();

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
final guestCustomerRemoteDataSourceProvider =
    Provider<GuestCustomerRemoteDataSource>(
      (ref) => GuestCustomerRemoteDataSourceImpl(ref.watch(dioProvider)),
    );
final checkoutRepositoryProvider = Provider<CheckoutRepository>(
  (ref) => CheckoutRepositoryImpl(
    ref.watch(orderRemoteDataSourceProvider),
    ref.watch(shippingCompanyRemoteDataSourceProvider),
    ref.watch(pricingRemoteDataSourceProvider),
    ref.watch(guestCustomerRemoteDataSourceProvider),
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
final createGuestCustomerUseCaseProvider =
    Provider<CreateGuestCustomerUseCase>(
      (ref) => CreateGuestCustomerUseCase(ref.watch(checkoutRepositoryProvider)),
    );
final calculatePriceGuestUseCaseProvider =
    Provider<CalculatePriceGuestUseCase>(
      (ref) => CalculatePriceGuestUseCase(ref.watch(checkoutRepositoryProvider)),
    );
final completeGuestOrderUseCaseProvider = Provider<CompleteGuestOrderUseCase>(
  (ref) => CompleteGuestOrderUseCase(
    ref.watch(checkoutRepositoryProvider),
    ref.watch(cartRepositoryProvider),
  ),
);

/// Kullanıcının oturum açıp açmadığını, secure storage'da access token olup
/// olmadığına bakarak belirler. Checkout ekranı bunu, giriş yapmış kullanıcı
/// akışına (`currentCustomerProvider` → `GET /api/customers/me`) hiç
/// girmeden misafir dalına geçebilmek için kullanır — aksi halde token
/// yokken bu çağrı 401 döner ve `AuthInterceptor` kullanıcıyı zorla
/// `/login`'e yönlendirir.
final isLoggedInProvider = FutureProvider.autoDispose<bool>((ref) async {
  final token = await ref
      .watch(secureStorageServiceProvider)
      .getString(StorageKeys.accessToken);
  return token != null;
});

/// Checkout açıldığında ve sepet her değiştiğinde (`cartControllerProvider`
/// izlendiği için) backend'in gerçek fiyat hesaplamasını yeniden çeker.
/// Misafir dalında, misafir müşteri kaydı (`guestCustomerId`) henüz
/// oluşturulmadıysa hesaplama yapılamaz — bu durumda kullanıcıya bunu
/// belirten bir `ValidationFailure` döner.
final priceCalculationProvider =
    FutureProvider.autoDispose<Result<PriceCalculation>>((ref) async {
      final items = ref.watch(
        cartControllerProvider.select((state) => state.items),
      );
      final isLoggedIn = await ref.watch(isLoggedInProvider.future);
      if (isLoggedIn) {
        return ref.watch(calculatePriceUseCaseProvider)(items);
      }
      final guestCustomerId = ref.watch(
        checkoutControllerProvider.select((state) => state.guestCustomerId),
      );
      if (guestCustomerId == null) {
        return const Result.failure(
          ValidationFailure(
            'Fiyatı görmek için önce iletişim bilgilerinizi kaydedin.',
          ),
        );
      }
      return ref.watch(calculatePriceGuestUseCaseProvider)(
        guestCustomerId,
        items,
      );
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
    this.guestCustomerId,
    this.guestInfo,
    this.isCreatingGuestCustomer = false,
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

  /// Misafir checkout'ta `POST /api/guest-customers` çağrısı sonucu elde
  /// edilen id — fiyat hesaplama (`calculatePriceGuest`) ve sipariş
  /// oluşturma (`completeGuestOrder`) arasında taşınır.
  final String? guestCustomerId;

  /// Misafirin `GuestCheckoutForm`'a girdiği iletişim/adres bilgileri —
  /// sipariş oluşturulurken kullanılır.
  final GuestCheckoutInfo? guestInfo;

  /// Misafir bilgileri kaydedilirken (`POST /api/guest-customers`) `true`.
  final bool isCreatingGuestCustomer;
  final bool isSubmitting;
  final Order? order;
  final Failure? failure;

  CheckoutState copyWith({
    String? selectedAddressId,
    String? selectedShippingCompanyId,
    Object? guestCustomerId = _unset,
    Object? guestInfo = _unset,
    bool? isCreatingGuestCustomer,
    bool? isSubmitting,
    Order? order,
    Failure? failure,
    bool clearFailure = false,
  }) => CheckoutState(
    selectedAddressId: selectedAddressId ?? this.selectedAddressId,
    selectedShippingCompanyId:
        selectedShippingCompanyId ?? this.selectedShippingCompanyId,
    guestCustomerId: identical(guestCustomerId, _unset)
        ? this.guestCustomerId
        : guestCustomerId as String?,
    guestInfo: identical(guestInfo, _unset)
        ? this.guestInfo
        : guestInfo as GuestCheckoutInfo?,
    isCreatingGuestCustomer:
        isCreatingGuestCustomer ?? this.isCreatingGuestCustomer,
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

  /// Misafir checkout'un ilk adımı: `GuestCheckoutForm` onaylandığında
  /// çağrılır, `POST /api/guest-customers` ile misafir müşteri kaydı açar.
  /// Dönen id, fiyat önizlemesi ve sipariş oluşturma için state'te tutulur.
  Future<void> submitGuestInfo(GuestCheckoutInfo info) async {
    state = state.copyWith(isCreatingGuestCustomer: true, clearFailure: true);
    final result = await ref.read(createGuestCustomerUseCaseProvider)(info);
    state = switch (result) {
      Success<String>(:final value) => state.copyWith(
        isCreatingGuestCustomer: false,
        guestCustomerId: value,
        guestInfo: info,
      ),
      ResultFailure<String>(:final failure) => state.copyWith(
        isCreatingGuestCustomer: false,
        failure: failure,
      ),
    };
  }

  Future<void> completeGuestOrder(List<CartItem> items) async {
    final guestCustomerId = state.guestCustomerId;
    final guestInfo = state.guestInfo;
    if (guestCustomerId == null || guestInfo == null) {
      state = state.copyWith(
        failure: const ValidationFailure(
          'Lütfen önce iletişim bilgilerinizi kaydedin.',
        ),
      );
      return;
    }
    state = state.copyWith(isSubmitting: true, clearFailure: true);
    final anonymousId = await ref
        .read(anonymousIdServiceProvider)
        .getOrCreateAnonymousId();
    final result = await ref.read(completeGuestOrderUseCaseProvider)(
      guestCustomerId: guestCustomerId,
      anonymousId: anonymousId,
      shippingCompanyId: state.selectedShippingCompanyId,
      info: guestInfo,
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
