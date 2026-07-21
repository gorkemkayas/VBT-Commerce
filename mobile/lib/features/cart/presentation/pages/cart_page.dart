import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../domain/entities/cart_item.dart';
import '../providers/cart_providers.dart';

class CartPage extends ConsumerWidget {
  const CartPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(cartControllerProvider);
    final controller = ref.read(cartControllerProvider.notifier);
    return Scaffold(
      appBar: AppBar(
        title: const Text('Sepetim'),
        actions: [
          if (state.items.isNotEmpty)
            TextButton(
              onPressed: controller.clear,
              child: const Text('Temizle'),
            ),
        ],
      ),
      body: switch (state) {
        CartState(isLoading: true, items: []) => const LoadingView(
          message: 'Sepet yükleniyor...',
        ),
        CartState(failure: final failure?, items: []) => ErrorView(
          message: failure.message,
          onRetry: controller.loadCart,
        ),
        CartState(items: []) => EmptyView(
          message: 'Sepetiniz boş.',
          onRetry: controller.loadCart,
        ),
        CartState(items: final items) => Column(
          children: [
            Expanded(
              child: ListView.separated(
                padding: const EdgeInsets.all(16),
                itemCount: items.length,
                separatorBuilder: (_, _) => const SizedBox(height: 12),
                itemBuilder: (context, index) =>
                    _CartItemTile(item: items[index]),
              ),
            ),
            _CartSummary(itemCount: state.itemCount, subtotal: state.subtotal),
          ],
        ),
      },
    );
  }
}

class _CartItemTile extends ConsumerWidget {
  const _CartItemTile({required this.item});
  final CartItem item;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final controller = ref.read(cartControllerProvider.notifier);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(
              height: 92,
              width: 72,
              child: AppNetworkImage(imageUrl: item.imageUrl),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.title,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: Theme.of(context).textTheme.titleSmall,
                  ),
                  const SizedBox(height: 6),
                  Text(
                    item.unitPrice > 0
                        ? '\$${item.unitPrice.toStringAsFixed(2)}'
                        : 'Fiyat yakında',
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      IconButton(
                        tooltip: 'Azalt',
                        onPressed: () => controller.updateQuantity(
                          itemId: item.id,
                          quantity: item.quantity - 1,
                        ),
                        icon: const Icon(Icons.remove_circle_outline),
                      ),
                      Text('${item.quantity}'),
                      IconButton(
                        tooltip: 'Artır',
                        onPressed: () => controller.updateQuantity(
                          itemId: item.id,
                          quantity: item.quantity + 1,
                        ),
                        icon: const Icon(Icons.add_circle_outline),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                IconButton(
                  tooltip: 'Ürünü kaldır',
                  onPressed: () => controller.remove(item.id),
                  icon: const Icon(Icons.delete_outline),
                ),
                Text(
                  item.unitPrice > 0
                      ? '\$${item.lineTotal.toStringAsFixed(2)}'
                      : 'Fiyat yakında',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _CartSummary extends StatelessWidget {
  const _CartSummary({required this.itemCount, required this.subtotal});
  final int itemCount;
  final double subtotal;

  @override
  Widget build(BuildContext context) => Material(
    elevation: 6,
    child: SafeArea(
      top: false,
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          children: [
            _SummaryRow(label: 'Toplam Ürün Sayısı', value: '$itemCount'),
            const SizedBox(height: 8),
            _SummaryRow(
              label: 'Ara Toplam',
              value: subtotal > 0
                  ? '\$${subtotal.toStringAsFixed(2)}'
                  : 'Fiyat yakında',
            ),
            const Divider(height: 24),
            _SummaryRow(
              label: 'Toplam Tutar',
              value: subtotal > 0
                  ? '\$${subtotal.toStringAsFixed(2)}'
                  : 'Fiyat yakında',
              emphasized: true,
            ),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: () => context.push(RoutePaths.checkout),
              child: const Text('Siparişi Tamamla'),
            ),
          ],
        ),
      ),
    ),
  );
}

class _SummaryRow extends StatelessWidget {
  const _SummaryRow({
    required this.label,
    required this.value,
    this.emphasized = false,
  });
  final String label;
  final String value;
  final bool emphasized;

  @override
  Widget build(BuildContext context) {
    final style = emphasized
        ? Theme.of(context).textTheme.titleLarge
        : Theme.of(context).textTheme.bodyLarge;
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: style),
        Text(value, style: style),
      ],
    );
  }
}
