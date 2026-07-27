import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/services/anonymous_id_service.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../../core/services/storage_service.dart';
import '../../data/datasources/coupon_remote_data_source.dart';
import '../../data/datasources/guest_customer_remote_data_source.dart';
import '../../data/datasources/order_remote_data_source.dart';
import '../../data/datasources/pricing_remote_data_source.dart';
import '../../data/datasources/shipping_company_remote_data_source.dart';
import '../../data/repositories/checkout_repository_impl.dart';
import '../../domain/entities/coupon.dart';
import '../../domain/entities/guest_billing_info.dart';
import '../../domain/entities/guest_checkout_info.dart';
import '../../domain/entities/guest_contact.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/payment_card_info.dart';
import '../../domain/entities/price_calculation.dart';
import '../../domain/entities/shipping_company.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../../domain/usecases/calculate_price_guest_use_case.dart';
import '../../domain/usecases/calculate_price_use_case.dart';
import '../../domain/usecases/complete_guest_order_use_case.dart';
import '../../domain/usecases/complete_order_use_case.dart';
import '../../domain/usecases/create_guest_customer_use_case.dart';
import '../../domain/usecases/get_active_coupons_use_case.dart';
import '../../domain/usecases/get_guest_customer_use_case.dart';
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
final couponRemoteDataSourceProvider = Provider<CouponRemoteDataSource>(
  (ref) => CouponRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final checkoutRepositoryProvider = Provider<CheckoutRepository>(
  (ref) => CheckoutRepositoryImpl(
    ref.watch(orderRemoteDataSourceProvider),
    ref.watch(shippingCompanyRemoteDataSourceProvider),
    ref.watch(pricingRemoteDataSourceProvider),
    ref.watch(guestCustomerRemoteDataSourceProvider),
    ref.watch(couponRemoteDataSourceProvider),
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
final getGuestCustomerUseCaseProvider = Provider<GetGuestCustomerUseCase>(
  (ref) => GetGuestCustomerUseCase(ref.watch(checkoutRepositoryProvider)),
);
final getActiveCouponsUseCaseProvider = Provider<GetActiveCouponsUseCase>(
  (ref) => GetActiveCouponsUseCase(ref.watch(checkoutRepositoryProvider)),
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
      // Uygulanan kupon kodları değiştiğinde de yeniden hesaplanır.
      final couponCodes = ref.watch(
        checkoutControllerProvider.select((state) => state.couponCodes),
      );
      final isLoggedIn = await ref.watch(isLoggedInProvider.future);
      if (isLoggedIn) {
        return ref.watch(calculatePriceUseCaseProvider)(items, couponCodes);
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
        couponCodes,
      );
    });

/// Aktif kargo firmalarının tamamını çeker; seçim UI'ı bunu izler.
final shippingCompaniesProvider =
    FutureProvider.autoDispose<Result<List<ShippingCompany>>>((ref) {
      return ref.watch(getShippingCompaniesUseCaseProvider)();
    });

/// Vitrine açık aktif kuponlar (`GET /api/coupons/active`) — kupon alanı
/// bunları "uygulanabilir kuponlar" olarak listeler. Hata durumunda boş liste
/// döner: kupon önerileri yardımcı bir özellik, checkout'u bloklamamalı.
final activeCouponsProvider = FutureProvider.autoDispose<List<Coupon>>((
  ref,
) async {
  final result = await ref.watch(getActiveCouponsUseCaseProvider)();
  return switch (result) {
    Success<List<Coupon>>(:final value) => value,
    ResultFailure<List<Coupon>>() => const <Coupon>[],
  };
});

/// Cihazda saklanan misafir müşteri id'si varsa (`StorageKeys.guestCustomerId`,
/// önceki bir misafir checkout'undan) o kaydın iletişim bilgilerini
/// `GET /api/guest-customers/{id}` ile çeker; misafir formu bunlarla önceden
/// doldurulur. Kayıt yoksa ya da sunucuda bulunamıyorsa (silinmiş id) `null`
/// döner ve form boş açılır.
final savedGuestContactProvider = FutureProvider.autoDispose<GuestContact?>((
  ref,
) async {
  final storedId = ref
      .watch(storageServiceProvider)
      .getString(StorageKeys.guestCustomerId);
  if (storedId == null || storedId.isEmpty) return null;
  final result = await ref.watch(getGuestCustomerUseCaseProvider)(storedId);
  return switch (result) {
    Success<GuestContact>(:final value) => value,
    ResultFailure<GuestContact>() => null,
  };
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
    this.couponCodes = const [],
    this.isApplyingCoupon = false,
    this.couponError,
    this.isCreatingGuestCustomer = false,
    this.sameBillingAddress = true,
    this.billingAddressId,
    this.guestBillingInfo,
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

  /// Kullanıcının uyguladığı (backend'ce doğrulanmış) kupon kodları. Yalnızca
  /// geçerli çıkan kodlar buraya eklenir (bkz. `applyCoupon`); fiyat hesaplama
  /// ve sipariş oluşturma bu listeyi kullanır.
  final List<String> couponCodes;

  /// Bir kupon uygulanırken (doğrulama isteği sürerken) `true`.
  final bool isApplyingCoupon;

  /// Son kupon denemesinin hata mesajı (geçersiz/süresi geçmiş kod vb.);
  /// başarılı uygulama ya da kaldırma bunu temizler.
  final String? couponError;

  /// Misafir bilgileri kaydedilirken (`POST /api/guest-customers`) `true`.
  final bool isCreatingGuestCustomer;

  /// `true` (varsayılan) ise fatura adresi teslimat adresiyle aynı kabul
  /// edilir ve `billingAddressId`/`guestBillingInfo` sipariş oluşturmaya
  /// gönderilmez (backend bu alanları opsiyonel tutar).
  final bool sameBillingAddress;

  /// Giriş yapmış kullanıcı için seçilen fatura adresi — Customer
  /// feature'ındaki `isBillingAddress` işaretli adreslerden biri.
  final String? billingAddressId;

  /// Misafir için ayrıca girilen fatura adresi.
  final GuestBillingInfo? guestBillingInfo;

  final bool isSubmitting;
  final Order? order;
  final Failure? failure;

  CheckoutState copyWith({
    String? selectedAddressId,
    String? selectedShippingCompanyId,
    Object? guestCustomerId = _unset,
    Object? guestInfo = _unset,
    List<String>? couponCodes,
    bool? isApplyingCoupon,
    Object? couponError = _unset,
    bool? isCreatingGuestCustomer,
    bool? sameBillingAddress,
    Object? billingAddressId = _unset,
    Object? guestBillingInfo = _unset,
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
    couponCodes: couponCodes ?? this.couponCodes,
    isApplyingCoupon: isApplyingCoupon ?? this.isApplyingCoupon,
    couponError: identical(couponError, _unset)
        ? this.couponError
        : couponError as String?,
    isCreatingGuestCustomer:
        isCreatingGuestCustomer ?? this.isCreatingGuestCustomer,
    sameBillingAddress: sameBillingAddress ?? this.sameBillingAddress,
    billingAddressId: identical(billingAddressId, _unset)
        ? this.billingAddressId
        : billingAddressId as String?,
    guestBillingInfo: identical(guestBillingInfo, _unset)
        ? this.guestBillingInfo
        : guestBillingInfo as GuestBillingInfo?,
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

  /// "Fatura adresim teslimat adresimle aynı" seçeneği — web'deki
  /// `sameBillingAddress` ile aynı davranış.
  void setSameBillingAddress(bool value) {
    state = state.copyWith(sameBillingAddress: value, clearFailure: true);
  }

  /// Giriş yapmış kullanıcı için fatura adresi seçimi (`isBillingAddress`
  /// işaretli adreslerden biri).
  void selectBillingAddress(String addressId) {
    state = state.copyWith(billingAddressId: addressId, clearFailure: true);
  }

  /// Misafir için fatura adresi bilgisi. `null` verilirse (form henüz
  /// doldurulmadıysa) sipariş oluşturma bunu ister.
  void setGuestBillingInfo(GuestBillingInfo? info) {
    state = state.copyWith(guestBillingInfo: info, clearFailure: true);
  }

  /// Bir kupon kodunu uygular. Kodu doğrudan state'e eklemek yerine önce
  /// backend'de doğrulanır (aday kod listesiyle fiyat hesaplanır): geçerliyse
  /// kod listeye eklenir ve `priceCalculationProvider` indirimi gösterir;
  /// geçersizse (`CouponNotFound`, süresi geçmiş, min. sepet tutarı vb.) kod
  /// eklenmez ve backend'in mesajı `couponError`'da gösterilir. Böylece hatalı
  /// bir kod, ödeme özetinin tamamını bozmaz.
  Future<void> applyCoupon(String rawCode) async {
    final code = rawCode.trim();
    if (code.isEmpty) return;
    if (state.couponCodes.any((c) => c.toLowerCase() == code.toLowerCase())) {
      state = state.copyWith(couponError: 'Bu kupon zaten uygulandı.');
      return;
    }

    state = state.copyWith(isApplyingCoupon: true, couponError: null);
    final candidate = [...state.couponCodes, code];
    final result = await _calculate(candidate);
    state = switch (result) {
      Success<PriceCalculation>() => state.copyWith(
        couponCodes: candidate,
        isApplyingCoupon: false,
        couponError: null,
      ),
      ResultFailure<PriceCalculation>(:final failure) => state.copyWith(
        isApplyingCoupon: false,
        couponError: failure.message,
      ),
    };
  }

  void removeCoupon(String code) {
    state = state.copyWith(
      couponCodes: state.couponCodes
          .where((c) => c != code)
          .toList(growable: false),
      couponError: null,
    );
  }

  /// Verilen kupon adaylarıyla fiyat hesaplar — `applyCoupon`'un doğrulama
  /// adımı. Giriş yapmış/misafir dalını `priceCalculationProvider` ile aynı
  /// şekilde ayırır.
  Future<Result<PriceCalculation>> _calculate(List<String> couponCodes) async {
    final items = ref.read(cartControllerProvider).items;
    final isLoggedIn = await ref.read(isLoggedInProvider.future);
    if (isLoggedIn) {
      return ref.read(calculatePriceUseCaseProvider)(items, couponCodes);
    }
    final guestCustomerId = state.guestCustomerId;
    if (guestCustomerId == null) {
      return const Result.failure(
        ValidationFailure(
          'Kupon uygulamak için önce iletişim bilgilerinizi kaydedin.',
        ),
      );
    }
    return ref.read(calculatePriceGuestUseCaseProvider)(
      guestCustomerId,
      items,
      couponCodes,
    );
  }

  Future<void> completeOrder(
    List<CartItem> items, {
    required PaymentCardInfo cardInfo,
  }) async {
    // Web'deki aynı kontrol: "aynı adres" kapalıyken bir fatura adresi
    // seçilmemişse sipariş denemeden önce durdurulur.
    if (!state.sameBillingAddress && state.billingAddressId == null) {
      state = state.copyWith(
        failure: const ValidationFailure('Lütfen bir fatura adresi seçin.'),
      );
      return;
    }
    state = state.copyWith(isSubmitting: true, clearFailure: true);
    final result = await ref.read(completeOrderUseCaseProvider)(
      addressId: state.selectedAddressId,
      billingAddressId: state.sameBillingAddress
          ? null
          : state.billingAddressId,
      shippingCompanyId: state.selectedShippingCompanyId,
      items: items,
      couponCodes: state.couponCodes,
      cardInfo: cardInfo,
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
  /// Dönen id, fiyat önizlemesi ve sipariş oluşturma için state'te tutulur;
  /// ayrıca cihazda saklanır — aynı cihazdaki bir sonraki misafir
  /// checkout'unda form bu kayıttan doldurulur (bkz.
  /// `savedGuestContactProvider`).
  Future<void> submitGuestInfo(GuestCheckoutInfo info) async {
    state = state.copyWith(isCreatingGuestCustomer: true, clearFailure: true);
    final result = await ref.read(createGuestCustomerUseCaseProvider)(info);
    if (result case Success<String>(:final value)) {
      await ref
          .read(storageServiceProvider)
          .setString(StorageKeys.guestCustomerId, value);
    }
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

  Future<void> completeGuestOrder(
    List<CartItem> items, {
    required PaymentCardInfo cardInfo,
  }) async {
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
    if (!state.sameBillingAddress && state.guestBillingInfo == null) {
      state = state.copyWith(
        failure: const ValidationFailure(
          'Lütfen fatura adresi alanlarını doldurun.',
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
      billingInfo: state.sameBillingAddress ? null : state.guestBillingInfo,
      items: items,
      couponCodes: state.couponCodes,
      cardInfo: cardInfo,
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
