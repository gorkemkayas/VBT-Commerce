import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../checkout/domain/entities/shipping_company.dart';
import '../../../checkout/presentation/providers/checkout_providers.dart';
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
          Success<Order>(:final value) => _OrderDetailBody(
            orderId: orderId,
            order: value,
          ),
          ResultFailure<Order>(:final failure) => ErrorView(
            message: failure.message,
            onRetry: () => ref.invalidate(orderDetailProvider(orderId)),
          ),
        },
      ),
    );
  }
}

/// Sipariş, `Pending` veya `Confirmed` durumundayken iptal edilebilir
/// (bkz. `OrderOperations.CancelAsync`) — bu iki durum dışında backend zaten
/// `400` ile reddediyor, bu yüzden buton yalnızca bu durumlarda gösterilir.
bool _isCancellable(String status) =>
    status == 'Pending' || status == 'Confirmed';

class _OrderDetailBody extends ConsumerStatefulWidget {
  const _OrderDetailBody({required this.orderId, required this.order});
  final String orderId;
  final Order order;

  @override
  ConsumerState<_OrderDetailBody> createState() => _OrderDetailBodyState();
}

class _OrderDetailBodyState extends ConsumerState<_OrderDetailBody> {
  bool _isCancelling = false;

  Future<void> _confirmCancel() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Siparişi iptal et'),
        content: const Text(
          'Bu siparişi iptal etmek istediğinize emin misiniz?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(false),
            child: const Text('Vazgeç'),
          ),
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(true),
            child: const Text('İptal Et'),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;

    setState(() => _isCancelling = true);
    final result = await ref.read(cancelOrderUseCaseProvider)(
      widget.orderId,
    );
    if (!mounted) return;
    setState(() => _isCancelling = false);

    switch (result) {
      case Success<bool>():
        // Sipariş detayı (durumu) ve sipariş listesi güncel bilgiyi
        // göstersin diye yeniden yüklenmeye zorlanır.
        ref.invalidate(orderDetailProvider(widget.orderId));
        ref.invalidate(ordersControllerProvider);
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(const SnackBar(content: Text('Siparişiniz iptal edildi.')));
      case ResultFailure<bool>(:final failure):
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  @override
  Widget build(BuildContext context) {
    final order = widget.order;
    return SingleChildScrollView(
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
          if (_isCancellable(order.status)) ...[
            const SizedBox(height: 24),
            OutlinedButton.icon(
              onPressed: _isCancelling ? null : _confirmCancel,
              icon: _isCancelling
                  ? const SizedBox(
                      height: 18,
                      width: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.cancel_outlined),
              label: const Text('Siparişi İptal Et'),
              style: OutlinedButton.styleFrom(
                foregroundColor: Theme.of(context).colorScheme.error,
              ),
            ),
          ],
        ],
      ),
    );
  }
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

/// `OrderDto` kargo firmasının yalnızca id'sini taşıyor, adını taşımıyor.
/// Ad, Checkout feature'ın (aktif firmaları listeleyen) mevcut
/// `shippingCompaniesProvider`'ı üzerinden çözülür — sipariş verildikten
/// sonra firma pasife alınmışsa (artık aktif listede yoksa) genel bir
/// etikete düşülür.
class _ShippingCompanyName extends ConsumerWidget {
  const _ShippingCompanyName({required this.shippingCompanyId, this.style});
  final String shippingCompanyId;
  final TextStyle? style;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final companies = ref.watch(shippingCompaniesProvider);
    return companies.when(
      data: (result) => Text(_resolveName(result), style: style),
      loading: () => Text('Yükleniyor...', style: style),
      error: (_, _) => Text('Kargo Firması', style: style),
    );
  }

  String _resolveName(Result<List<ShippingCompany>> result) {
    final companies = switch (result) {
      Success<List<ShippingCompany>>(:final value) => value,
      ResultFailure<List<ShippingCompany>>() => const <ShippingCompany>[],
    };
    for (final company in companies) {
      if (company.id == shippingCompanyId) return company.name;
    }
    return 'Kargo Firması';
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
            if (order.shippingCompanyId != null)
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 2),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    _ShippingCompanyName(
                      shippingCompanyId: order.shippingCompanyId!,
                      style: bodyStyle,
                    ),
                    Text(order.shippingFee.toTryCurrency(), style: bodyStyle),
                  ],
                ),
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
