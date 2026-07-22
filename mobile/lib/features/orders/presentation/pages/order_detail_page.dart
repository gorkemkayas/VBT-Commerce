import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../product/domain/entities/product.dart';
import '../../../product/presentation/providers/product_providers.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/order_address.dart';
import '../../domain/entities/order_item.dart';
import '../providers/order_providers.dart';

final _dateFormat = DateFormat('d MMMM y, HH:mm', 'tr_TR');

class OrderDetailPage extends ConsumerWidget {
  const OrderDetailPage({super.key, required this.orderId});
  final String orderId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final order = ref.watch(orderDetailProvider(orderId));
    return Scaffold(
      appBar: AppBar(title: const Text('Sipariş Detayı')),
      body: order.when(
        loading: () => const LoadingView(message: 'Sipariş yükleniyor...'),
        error: (_, _) => ErrorView(
          message: 'Sipariş yüklenemedi.',
          onRetry: () => ref.invalidate(orderDetailProvider(orderId)),
        ),
        data: (result) => switch (result) {
          Success<Order>(:final value) => _OrderDetailBody(order: value),
          ResultFailure<Order>(:final failure) => ErrorView(
            message: failure.message,
            onRetry: () => ref.invalidate(orderDetailProvider(orderId)),
          ),
        },
      ),
    );
  }
}

class _OrderDetailBody extends StatelessWidget {
  const _OrderDetailBody({required this.order});
  final Order order;

  @override
  Widget build(BuildContext context) => SingleChildScrollView(
    padding: const EdgeInsets.all(16),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _HeaderCard(order: order),
        const SizedBox(height: 16),
        if (order.address != null) ...[
          _AddressCard(address: order.address!),
          const SizedBox(height: 16),
        ],
        _ItemsCard(items: order.items),
        const SizedBox(height: 16),
        _TotalsCard(order: order),
      ],
    ),
  );
}

class _HeaderCard extends StatelessWidget {
  const _HeaderCard({required this.order});
  final Order order;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(_shortId(order.id), style: theme.textTheme.titleMedium),
                _StatusChip(status: order.status),
              ],
            ),
            const SizedBox(height: 4),
            Text(
              _dateFormat.format(order.createdAt),
              style: theme.textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }

  static String _shortId(String id) => '#${id.split('-').first.toUpperCase()}';
}

class _StatusChip extends StatelessWidget {
  const _StatusChip({required this.status});
  final String status;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = theme.colorScheme;
    final (label, color) = switch (status) {
      'Pending' => ('Beklemede', colors.tertiary),
      'Confirmed' => ('Onaylandı', colors.primary),
      'Cancelled' => ('İptal edildi', colors.error),
      _ => (status, colors.outline),
    };
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: color.withValues(alpha: .12),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        label,
        style: theme.textTheme.labelSmall?.copyWith(color: color),
      ),
    );
  }
}

class _AddressCard extends StatelessWidget {
  const _AddressCard({required this.address});
  final OrderAddress address;

  @override
  Widget build(BuildContext context) => Card(
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
          Text(address.recipientName),
          Text(address.phoneNumber),
          Text(_summary(address)),
        ],
      ),
    ),
  );

  String _summary(OrderAddress address) {
    final line2 = address.addressLine2;
    final addressLine = line2 == null || line2.isEmpty
        ? address.addressLine1
        : '${address.addressLine1}, $line2';
    return '$addressLine, ${address.district}/${address.city}, '
        '${address.country} ${address.postalCode}';
  }
}

class _ItemsCard extends StatelessWidget {
  const _ItemsCard({required this.items});
  final List<OrderItem> items;

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Ürünler', style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: 12),
          for (final item in items) ...[
            _OrderItemRow(item: item),
            const SizedBox(height: 8),
          ],
        ],
      ),
    ),
  );
}

/// `OrderItemDto` ürün adı taşımıyor. `Product` tipindeki kalemlerde ad,
/// Product feature'ın mevcut `productDetailProvider`'ı üzerinden çözülür.
/// `Variant` tipinde ise variant id'sinden ürüne geri dönen bir endpoint
/// olmadığı için (bkz. görev analizi) genel bir etiket gösterilir.
class _OrderItemRow extends ConsumerWidget {
  const _OrderItemRow({required this.item});
  final OrderItem item;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final style = Theme.of(context).textTheme.bodyMedium;
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          child: item.sellableItemType == 'Product'
              ? _ProductName(productId: item.sellableItemId, style: style)
              : Text('Ürün varyantı', style: style),
        ),
        Text('x${item.quantity}', style: style),
        const SizedBox(width: 12),
        SizedBox(
          width: 90,
          child: Text(
            item.lineSubtotal.toTryCurrency(),
            textAlign: TextAlign.right,
            style: style,
          ),
        ),
      ],
    );
  }
}

class _ProductName extends ConsumerWidget {
  const _ProductName({required this.productId, this.style});
  final String productId;
  final TextStyle? style;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final product = ref.watch(productDetailProvider(productId));
    return product.when(
      data: (result) => Text(
        switch (result) {
          Success<Product>(:final value) => value.title,
          ResultFailure<Product>() => 'Ürün',
        },
        maxLines: 2,
        overflow: TextOverflow.ellipsis,
        style: style,
      ),
      loading: () => Text('Yükleniyor...', style: style),
      error: (_, _) => Text('Ürün', style: style),
    );
  }
}

class _TotalsCard extends StatelessWidget {
  const _TotalsCard({required this.order});
  final Order order;

  @override
  Widget build(BuildContext context) {
    final bodyStyle = Theme.of(context).textTheme.bodyLarge;
    final totalStyle = Theme.of(context).textTheme.titleLarge;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _SummaryRow(
              label: 'Ara Toplam',
              value: order.subtotal,
              style: bodyStyle,
            ),
            if (order.discountAmount > 0)
              _SummaryRow(
                label: 'İndirim',
                value: -order.discountAmount,
                style: bodyStyle,
              ),
            _SummaryRow(
              label: 'Vergi',
              value: order.taxAmount,
              style: bodyStyle,
            ),
            const Divider(height: 24),
            _SummaryRow(
              label: 'Genel Toplam',
              value: order.totalAmount,
              style: totalStyle,
            ),
          ],
        ),
      ),
    );
  }
}

class _SummaryRow extends StatelessWidget {
  const _SummaryRow({required this.label, required this.value, this.style});
  final String label;
  final double value;
  final TextStyle? style;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.symmetric(vertical: 2),
    child: Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: style),
        Text(value.toTryCurrency(), style: style),
      ],
    ),
  );
}
