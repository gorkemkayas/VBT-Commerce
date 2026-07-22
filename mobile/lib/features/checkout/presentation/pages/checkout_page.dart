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
import '../providers/checkout_providers.dart';
import '../widgets/address_selector.dart';
import '../widgets/complete_order_button.dart';
import '../widgets/order_summary_view.dart';
import '../widgets/payment_summary_view.dart';

class CheckoutPage extends ConsumerWidget {
  const CheckoutPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final cartState = ref.watch(cartControllerProvider);
    final checkoutState = ref.watch(checkoutControllerProvider);
    final controller = ref.read(checkoutControllerProvider.notifier);
    final customerResult = ref.watch(currentCustomerProvider);

    ref.listen(checkoutControllerProvider, (previous, next) {
      if (next.order != null && previous?.order == null) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Siparişiniz alındı: ${next.order!.orderId}')),
        );
        context.go(RoutePaths.home);
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
                  customerResult.when(
                    loading: () => const Padding(
                      padding: EdgeInsets.symmetric(vertical: 24),
                      child: Center(child: CircularProgressIndicator()),
                    ),
                    error: (_, _) => const _AddressLoadError(),
                    data: (result) => switch (result) {
                      Success<Customer>(:final value) => _AddressSection(
                        addresses: value.addresses,
                        selectedAddressId: checkoutState.selectedAddressId,
                        onSelected: controller.selectAddress,
                      ),
                      ResultFailure<Customer>() => const _AddressLoadError(),
                    },
                  ),
                  const SizedBox(height: 16),
                  PaymentSummaryView(subtotal: cartState.subtotal),
                  const SizedBox(height: 24),
                  CompleteOrderButton(
                    isSubmitting: checkoutState.isSubmitting,
                    enabled: checkoutState.selectedAddressId != null,
                    onPressed: () => controller.completeOrder(cartState.items),
                  ),
                ],
              ),
            ),
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
