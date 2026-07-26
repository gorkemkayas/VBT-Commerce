import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/order_address.dart';
import '../../domain/entities/order_item.dart';
import '../providers/order_providers.dart';

final _dateFormat = DateFormat('d MMMM y, HH:mm', 'tr_TR');

/// Misafir sipariş sorgulama — web'deki `/track-order` (`TrackOrderForm`) ile
/// aynı işleyiş: kullanıcı checkout onayında kendisine gösterilen sipariş no
/// ve müşteri no ikilisini girer, `GET /api/orders/guest/{guestCustomerId}/
/// {orderId}` (oturum gerektirmez) ile sipariş görüntülenir. Web'in aksine
/// kargo takibi burada gösterilmez — backend'de misafir siparişleri için
/// shipment endpoint'i yok (bkz. `GuestOrdersController`), web de aynı
/// nedenle bu bölümü misafir dalında hiç çekmiyor.
class GuestOrderLookupPage extends ConsumerStatefulWidget {
  const GuestOrderLookupPage({super.key});

  @override
  ConsumerState<GuestOrderLookupPage> createState() =>
      _GuestOrderLookupPageState();
}

class _GuestOrderLookupPageState extends ConsumerState<GuestOrderLookupPage> {
  final _formKey = GlobalKey<FormState>();
  final _orderIdController = TextEditingController();
  final _guestCustomerIdController = TextEditingController();
  GuestOrderLookupKey? _submitted;

  @override
  void dispose() {
    _orderIdController.dispose();
    _guestCustomerIdController.dispose();
    super.dispose();
  }

  String? _validateRequired(String? value) =>
      value == null || value.trim().isEmpty ? 'Bu alan zorunludur.' : null;

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    setState(() {
      _submitted = (
        orderId: _orderIdController.text.trim(),
        guestCustomerId: _guestCustomerIdController.text.trim(),
      );
    });
  }

  void _reset() {
    setState(() => _submitted = null);
  }

  @override
  Widget build(BuildContext context) {
    final submitted = _submitted;
    return Scaffold(
      appBar: AppBar(title: const Text('Siparişimi Bul')),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (submitted == null) ...[
                  Text(
                    'Misafir Sipariş Sorgulama',
                    style: Theme.of(context).textTheme.headlineSmall,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Sipariş onayında size gösterilen sipariş no ve müşteri '
                    'no ikilisini girin.',
                    style: Theme.of(context).textTheme.bodyMedium,
                  ),
                  const SizedBox(height: 24),
                  Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        TextFormField(
                          controller: _orderIdController,
                          decoration: const InputDecoration(
                            labelText: 'Sipariş No',
                          ),
                          validator: _validateRequired,
                        ),
                        const SizedBox(height: 16),
                        TextFormField(
                          controller: _guestCustomerIdController,
                          decoration: const InputDecoration(
                            labelText: 'Müşteri No',
                          ),
                          validator: _validateRequired,
                        ),
                        const SizedBox(height: 24),
                        FilledButton(
                          onPressed: _submit,
                          child: const Text('Siparişi Görüntüle'),
                        ),
                      ],
                    ),
                  ),
                ] else ...[
                  _GuestOrderResult(target: submitted),
                  const SizedBox(height: 16),
                  TextButton(
                    onPressed: _reset,
                    child: const Text('Başka bir sipariş sorgula'),
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _GuestOrderResult extends ConsumerWidget {
  const _GuestOrderResult({required this.target});
  final GuestOrderLookupKey target;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final order = ref.watch(guestOrderDetailProvider(target));
    return order.when(
      loading: () => const LoadingView(message: 'Sipariş aranıyor...'),
      error: (_, _) => ErrorView(
        message: 'Sipariş bulunamadı.',
        onRetry: () => ref.invalidate(guestOrderDetailProvider(target)),
      ),
      data: (result) => switch (result) {
        Success<Order>(:final value) => _GuestOrderCard(order: value),
        ResultFailure<Order>(:final failure) => ErrorView(
          message: failure.message,
          onRetry: () => ref.invalidate(guestOrderDetailProvider(target)),
        ),
      },
    );
  }
}

class _GuestOrderCard extends StatelessWidget {
  const _GuestOrderCard({required this.order});
  final Order order;

  static String _shortId(String id) => '#${id.split('-').first.toUpperCase()}';

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final address = order.address;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(_shortId(order.id), style: theme.textTheme.titleMedium),
                    _OrderStatusChip(status: order.status),
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
        ),
        if (address != null) ...[
          const SizedBox(height: 16),
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Teslimat Adresi',
                    style: theme.textTheme.titleMedium,
                  ),
                  const SizedBox(height: 8),
                  Text(address.recipientName),
                  Text(address.phoneNumber),
                  Text(_addressSummary(address)),
                ],
              ),
            ),
          ),
        ],
        const SizedBox(height: 16),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Ürünler', style: theme.textTheme.titleMedium),
                const SizedBox(height: 12),
                for (final item in order.items) ...[
                  _GuestOrderItemRow(item: item),
                  const SizedBox(height: 8),
                ],
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _SummaryRow(label: 'Ara Toplam', value: order.subtotal),
                if (order.discountAmount > 0)
                  _SummaryRow(label: 'İndirim', value: -order.discountAmount),
                _SummaryRow(label: 'Vergi', value: order.taxAmount),
                if (order.shippingCompanyId != null)
                  _SummaryRow(label: 'Kargo', value: order.shippingFee),
                const Divider(height: 24),
                _SummaryRow(
                  label: 'Genel Toplam',
                  value: order.totalAmount,
                  emphasize: true,
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  String _addressSummary(OrderAddress address) {
    final line2 = address.addressLine2;
    final addressLine = line2 == null || line2.isEmpty
        ? address.addressLine1
        : '${address.addressLine1}, $line2';
    return '$addressLine, ${address.district}/${address.city}, '
        '${address.country} ${address.postalCode}';
  }
}

class _GuestOrderItemRow extends StatelessWidget {
  const _GuestOrderItemRow({required this.item});
  final OrderItem item;

  @override
  Widget build(BuildContext context) {
    final style = Theme.of(context).textTheme.bodyMedium;
    return Row(
      children: [
        Expanded(
          child: Text(
            item.sellableItemType == 'Product' ? 'Ürün' : 'Ürün varyantı',
            style: style,
          ),
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

class _SummaryRow extends StatelessWidget {
  const _SummaryRow({
    required this.label,
    required this.value,
    this.emphasize = false,
  });
  final String label;
  final double value;
  final bool emphasize;

  @override
  Widget build(BuildContext context) {
    final style = emphasize
        ? Theme.of(context).textTheme.titleLarge
        : Theme.of(context).textTheme.bodyLarge;
    return Padding(
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
}

class _OrderStatusChip extends StatelessWidget {
  const _OrderStatusChip({required this.status});
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
