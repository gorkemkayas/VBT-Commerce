import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../providers/checkout_providers.dart';
import '../widgets/address_form.dart';
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
                  AddressForm(
                    address: checkoutState.address,
                    onChanged: controller.updateAddress,
                  ),
                  const SizedBox(height: 16),
                  PaymentSummaryView(subtotal: cartState.subtotal),
                  const SizedBox(height: 24),
                  CompleteOrderButton(
                    isSubmitting: checkoutState.isSubmitting,
                    onPressed: () => controller.completeOrder(cartState.items),
                  ),
                ],
              ),
            ),
    );
  }
}
