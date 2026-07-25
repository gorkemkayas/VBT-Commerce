import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/result.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/shipment_tracking.dart';
import '../providers/order_providers.dart';
import '../widgets/shipping_info_widgets.dart';

final _dateFormat = DateFormat('d MMMM y', 'tr_TR');

/// Hesabım > Kargo Takibi — web'in `/account`'taki ayrı "Kargo Takibi" tab'ı
/// (`ShippingTab`) ile aynı amaç: Sipariş Detayı'na girmeden, aktif
/// (iptal edilmemiş) tüm siparişlerin kargo durumunu tek ekranda gösterir.
///
/// Mevcut `ordersControllerProvider`'ı (Siparişlerim ile aynı veri kaynağı)
/// ve `orderShipmentProvider`'ı (Sipariş Detayı'ndaki kargo kartıyla aynı
/// provider) yeniden kullanır — yeni bir repository/usecase gerekmez.
class ShipmentTrackingPage extends ConsumerWidget {
  const ShipmentTrackingPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(ordersControllerProvider);
    // Web'in `ShippingTab`'ıyla birebir aynı kural: iptal edilen siparişler
    // hiç listelenmez, shipment'ı bile sorgulanmaz — backend, sipariş iptal
    // edilse dahi ilişkili `Shipment` kaydını güncellemediği için (bkz.
    // `order_detail_page.dart`'taki `_ShipmentCard` açıklaması) aksi halde
    // eski/yanıltıcı bir durum gösterilmiş olurdu.
    final activeOrders = state.orders
        .where((order) => order.status != 'Cancelled')
        .toList();

    return Scaffold(
      appBar: AppBar(title: const Text('Kargo Takibi')),
      body: RefreshIndicator(
        onRefresh: () =>
            ref.read(ordersControllerProvider.notifier).loadOrders(),
        child: _buildBody(state, activeOrders),
      ),
    );
  }

  Widget _buildBody(OrdersState state, List<Order> activeOrders) {
    if (state.isLoading && state.orders.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (activeOrders.isEmpty) {
      // `AlwaysScrollableScrollPhysics`, liste boşken de aşağı çekip
      // yenilemeye izin verir.
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: 400,
            child: Center(
              child: Text(
                state.failure?.message ??
                    'Şu anda kargo sürecinde bir siparişiniz yok.',
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
      itemCount: activeOrders.length,
      separatorBuilder: (context, index) => const Divider(height: 1),
      itemBuilder: (context, index) =>
          _ShipmentOrderTile(order: activeOrders[index]),
    );
  }
}

class _ShipmentOrderTile extends ConsumerWidget {
  const _ShipmentOrderTile({required this.order});
  final Order order;

  static String _shortId(String id) => '#${id.split('-').first.toUpperCase()}';

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final shipmentResult = ref.watch(orderShipmentProvider(order.id)).value;
    final shipment = switch (shipmentResult) {
      Success<ShipmentTracking?>(:final value) => value,
      ResultFailure<ShipmentTracking?>() => null,
      null => null,
    };

    return ExpansionTile(
      title: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(_shortId(order.id), style: theme.textTheme.titleSmall),
              Text(
                _dateFormat.format(order.createdAt),
                style: theme.textTheme.bodySmall,
              ),
            ],
          ),
          if (shipment != null) ShipmentStatusChip(status: shipment.status),
        ],
      ),
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (shipment?.trackingNumber != null)
                Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: Text('Takip No: ${shipment!.trackingNumber}'),
                ),
              if (shipment != null && shipment.history.isNotEmpty)
                ShipmentTimeline(history: shipment.history)
              else
                Text(
                  'Bu sipariş için kargo süreci bilgisi henüz oluşturulmadı.',
                  style: theme.textTheme.bodySmall,
                ),
            ],
          ),
        ),
      ],
    );
  }
}
