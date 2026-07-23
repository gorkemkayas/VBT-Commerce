import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../customer/domain/entities/customer.dart';
import '../../../customer/domain/entities/customer_address.dart';
import '../../../customer/presentation/providers/customer_providers.dart';
import '../../../orders/presentation/providers/order_providers.dart';
import '../../domain/entities/guest_checkout_info.dart';
import '../../domain/entities/shipping_company.dart';
import '../providers/checkout_providers.dart';
import '../widgets/address_selector.dart';
import '../widgets/complete_order_button.dart';
import '../widgets/guest_checkout_form.dart';
import '../widgets/order_summary_view.dart';
import '../widgets/payment_summary_view.dart';
import '../widgets/shipping_company_selector.dart';

class CheckoutPage extends ConsumerWidget {
  const CheckoutPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final cartState = ref.watch(cartControllerProvider);
    final checkoutState = ref.watch(checkoutControllerProvider);
    final controller = ref.read(checkoutControllerProvider.notifier);
    final shippingCompaniesResult = ref.watch(shippingCompaniesProvider);
    // Misafir dalında `currentCustomerProvider` (`GET /api/customers/me`)
    // hiç çağrılmaz: token yokken bu istek 401 döner ve `AuthInterceptor`
    // kullanıcıyı zorla `/login`'e yönlendirir (`_forceLogout`). Bu yüzden
    // önce oturum durumu kontrol edilir.
    final loggedIn = ref.watch(isLoggedInProvider).value;

    ref.listen(checkoutControllerProvider, (previous, next) {
      if (next.order != null && previous?.order == null) {
        if (next.guestCustomerId != null) {
          // Misafir siparişi "Siparişlerim" listesine girmez (o liste
          // `/api/orders/me`'dir); bu yüzden sipariş no'su ayrı bir
          // diyalogda gösterilir. `ordersControllerProvider` de bilerek
          // invalidate edilmez — bu da `/api/orders/me`'yi tetikleyip aynı
          // 401/zorla-logout riskini doğururdu.
          showDialog<void>(
            context: context,
            builder: (dialogContext) => AlertDialog(
              title: const Text('Siparişiniz alındı'),
              content: Text(
                'Sipariş numaranız: ${next.order!.orderId}\n\n'
                'Misafir siparişi olduğu için "Siparişlerim" listenizde '
                'görünmeyecek, lütfen bu numarayı not edin.',
              ),
              actions: [
                TextButton(
                  onPressed: () => Navigator.of(dialogContext).pop(),
                  child: const Text('Tamam'),
                ),
              ],
            ),
          ).then((_) {
            if (context.mounted) context.go(RoutePaths.home);
          });
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Siparişiniz alındı: ${next.order!.orderId}'),
            ),
          );
          // "Siparişlerim" listesi kalıcı bir provider; yeni sipariş anında
          // görünsün diye sonraki açılışta yeniden yüklenmeye zorlanır.
          ref.invalidate(ordersControllerProvider);
          context.go(RoutePaths.home);
        }
      } else if (next.failure != null && next.failure != previous?.failure) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(next.failure!.message)));
      }
    });

    return Scaffold(
      appBar: AppBar(title: const Text('Ödeme')),
      body: cartState.items.isEmpty
          ? const EmptyView(message: 'Sepetiniz boş, önce ürün ekleyin.')
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  OrderSummaryView(items: cartState.items),
                  const SizedBox(height: 16),
                  _AddressOrGuestSection(
                    loggedIn: loggedIn,
                    selectedAddressId: checkoutState.selectedAddressId,
                    onAddressSelected: controller.selectAddress,
                    isSubmittingGuestInfo: checkoutState.isCreatingGuestCustomer,
                    isGuestInfoConfirmed: checkoutState.guestCustomerId != null,
                    onGuestInfoSubmitted: controller.submitGuestInfo,
                  ),
                  const SizedBox(height: 16),
                  shippingCompaniesResult.when(
                    loading: () => const Padding(
                      padding: EdgeInsets.symmetric(vertical: 24),
                      child: Center(child: CircularProgressIndicator()),
                    ),
                    error: (_, _) => const _ShippingLoadError(),
                    data: (result) => switch (result) {
                      Success<List<ShippingCompany>>(:final value) =>
                        _ShippingSection(
                          companies: value,
                          selectedShippingCompanyId:
                              checkoutState.selectedShippingCompanyId,
                          onSelected: controller.selectShippingCompany,
                        ),
                      ResultFailure<List<ShippingCompany>>() =>
                        const _ShippingLoadError(),
                    },
                  ),
                  const SizedBox(height: 16),
                  const PaymentSummaryView(),
                  const SizedBox(height: 24),
                  CompleteOrderButton(
                    isSubmitting: checkoutState.isSubmitting,
                    enabled: switch (loggedIn) {
                      true =>
                        checkoutState.selectedAddressId != null &&
                            checkoutState.selectedShippingCompanyId != null,
                      false =>
                        checkoutState.guestCustomerId != null &&
                            checkoutState.selectedShippingCompanyId != null,
                      null => false,
                    },
                    onPressed: () => loggedIn == true
                        ? controller.completeOrder(cartState.items)
                        : controller.completeGuestOrder(cartState.items),
                  ),
                ],
              ),
            ),
    );
  }
}

/// Oturum durumuna göre ya (giriş yapmış kullanıcı için, değişmeyen) kayıtlı
/// adres seçimini ya da misafir formunu gösterir. `currentCustomerProvider`
/// yalnızca `loggedIn == true` iken, bu widget'ın içinde watch edilir — bu
/// sayede misafir dalında `GET /api/customers/me` hiç çağrılmaz.
class _AddressOrGuestSection extends ConsumerWidget {
  const _AddressOrGuestSection({
    required this.loggedIn,
    required this.selectedAddressId,
    required this.onAddressSelected,
    required this.isSubmittingGuestInfo,
    required this.isGuestInfoConfirmed,
    required this.onGuestInfoSubmitted,
  });

  /// `null` iken oturum durumu henüz belirlenmedi (kısa süreli, secure
  /// storage okuması bitene kadar).
  final bool? loggedIn;
  final String? selectedAddressId;
  final ValueChanged<String> onAddressSelected;
  final bool isSubmittingGuestInfo;
  final bool isGuestInfoConfirmed;
  final ValueChanged<GuestCheckoutInfo> onGuestInfoSubmitted;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    if (loggedIn == null) {
      return const Padding(
        padding: EdgeInsets.symmetric(vertical: 24),
        child: Center(child: CircularProgressIndicator()),
      );
    }
    if (loggedIn == false) {
      return GuestCheckoutForm(
        isSubmitting: isSubmittingGuestInfo,
        isConfirmed: isGuestInfoConfirmed,
        onSubmit: onGuestInfoSubmitted,
      );
    }
    final customerResult = ref.watch(currentCustomerProvider);
    return customerResult.when(
      loading: () => const Padding(
        padding: EdgeInsets.symmetric(vertical: 24),
        child: Center(child: CircularProgressIndicator()),
      ),
      error: (_, _) => const _AddressLoadError(),
      data: (result) => switch (result) {
        Success<Customer>(:final value) => _AddressSection(
          addresses: value.addresses,
          selectedAddressId: selectedAddressId,
          onSelected: onAddressSelected,
        ),
        ResultFailure<Customer>() => const _AddressLoadError(),
      },
    );
  }
}

class _AddressSection extends StatelessWidget {
  const _AddressSection({
    required this.addresses,
    required this.selectedAddressId,
    required this.onSelected,
  });

  final List<CustomerAddress> addresses;
  final String? selectedAddressId;
  final ValueChanged<String> onSelected;

  @override
  Widget build(BuildContext context) {
    if (addresses.isEmpty) {
      return Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Teslimat Adresi',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              const SizedBox(height: 8),
              const Text('Henüz kayıtlı adresiniz yok.'),
              const SizedBox(height: 12),
              OutlinedButton.icon(
                onPressed: () => context.push(RoutePaths.addresses),
                icon: const Icon(Icons.add),
                label: const Text('Adres Ekle'),
              ),
            ],
          ),
        ),
      );
    }
    return AddressSelector(
      addresses: addresses,
      selectedAddressId: selectedAddressId,
      onSelected: onSelected,
    );
  }
}

class _AddressLoadError extends StatelessWidget {
  const _AddressLoadError();

  @override
  Widget build(BuildContext context) => const Card(
    child: Padding(
      padding: EdgeInsets.all(16),
      child: Text('Adresleriniz yüklenemedi. Lütfen tekrar deneyin.'),
    ),
  );
}

class _ShippingSection extends StatelessWidget {
  const _ShippingSection({
    required this.companies,
    required this.selectedShippingCompanyId,
    required this.onSelected,
  });

  final List<ShippingCompany> companies;
  final String? selectedShippingCompanyId;
  final ValueChanged<String> onSelected;

  @override
  Widget build(BuildContext context) {
    if (companies.isEmpty) {
      return Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Kargo Firması',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              const SizedBox(height: 8),
              const Text(
                'Şu anda kullanılabilir bir kargo firması bulunamadı.',
              ),
            ],
          ),
        ),
      );
    }
    return ShippingCompanySelector(
      companies: companies,
      selectedShippingCompanyId: selectedShippingCompanyId,
      onSelected: onSelected,
    );
  }
}

class _ShippingLoadError extends StatelessWidget {
  const _ShippingLoadError();

  @override
  Widget build(BuildContext context) => const Card(
    child: Padding(
      padding: EdgeInsets.all(16),
      child: Text('Kargo firmaları yüklenemedi. Lütfen tekrar deneyin.'),
    ),
  );
}
