import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../domain/entities/order.dart';
import '../providers/order_providers.dart';

final _dateFormat = DateFormat('d MMMM y, HH:mm', 'tr_TR');

class OrdersPage extends ConsumerWidget {
  const OrdersPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(ordersControllerProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Siparişlerim')),
      body: RefreshIndicator(
        onRefresh: () =>
            ref.read(ordersControllerProvider.notifier).loadOrders(),
        child: _buildBody(state),
      ),
    );
  }

  Widget _buildBody(OrdersState state) {
    if (state.isLoading && state.orders.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.orders.isEmpty) {
      // `AlwaysScrollableScrollPhysics`, liste boşken de aşağı çekip
      // yenilemeye izin verir.
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: 400,
            child: Center(
              child: Text(
                state.failure?.message ?? 'Henüz sipariş bulunmuyor.',
                textAlign: TextAlign.center,
              ),
            ),
          ),
        ],
      );
    }
    return ListView.separated(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.symmetric(vertical: 8),
      itemCount: state.orders.length,
      separatorBuilder: (context, index) => const Divider(height: 1),
      itemBuilder: (context, index) => _OrderTile(order: state.orders[index]),
    );
  }
}

class _OrderTile extends StatelessWidget {
  const _OrderTile({required this.order});
  final Order order;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return ListTile(
      onTap: () => context.push('/orders/${order.id}'),
      title: Text(
        _dateFormat.format(order.createdAt),
        style: theme.textTheme.titleSmall,
      ),
      subtitle: Padding(
        padding: const EdgeInsets.only(top: 4),
        child: Text(
          order.itemCount > 0
              ? '${order.itemCount} ürün · ${_shortId(order.id)}'
              : _shortId(order.id),
        ),
      ),
      trailing: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          Text(
            order.totalAmount.toTryCurrency(),
            style: theme.textTheme.titleMedium,
          ),
          const SizedBox(height: 4),
          _StatusChip(status: order.status),
        ],
      ),
    );
  }

  /// Sipariş id'si bir GUID; kullanıcıya tamamını göstermek yerine takip için
  /// yeterli olan ilk bloğu gösterilir.
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
