import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/result.dart';
import '../../../checkout/domain/entities/shipping_company.dart';
import '../../../checkout/presentation/providers/checkout_providers.dart';
import '../../domain/entities/shipment_status_history_entry.dart';

final _dateFormat = DateFormat('d MMMM y, HH:mm', 'tr_TR');

/// Backend'in `ShipmentStatus` değerinin Türkçe karşılığı — web'deki
/// `enum-labels.ts` → `shipmentStatusLabels` ile birebir aynı. Hem sipariş
/// detayındaki kargo kartında hem de "Kargo Takibi" ekranında kullanılır.
String shipmentStatusLabel(String status) => switch (status) {
  'Pending' => 'Hazırlanıyor',
  'Shipped' => 'Kargoya Verildi',
  'InTransit' => 'Dağıtımda',
  'Delivered' => 'Teslim Edildi',
  'Cancelled' => 'İptal Edildi',
  _ => status,
};

class ShipmentStatusChip extends StatelessWidget {
  const ShipmentStatusChip({super.key, required this.status});
  final String status;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = theme.colorScheme;
    final color = switch (status) {
      'Pending' => colors.tertiary,
      'Shipped' || 'InTransit' => colors.primary,
      'Delivered' => colors.secondary,
      'Cancelled' => colors.error,
      _ => colors.outline,
    };
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: color.withValues(alpha: .12),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        shipmentStatusLabel(status),
        style: theme.textTheme.labelSmall?.copyWith(color: color),
      ),
    );
  }
}

/// Backend'in `History` dizisinin sırasına güvenilir (bkz. web'in
/// `ShipmentTimeline`'ı — aynı varsayımla en son kaydı "güncel durum" olarak
/// vurgular); burada da aynı davranış birebir uygulanır.
class ShipmentTimeline extends StatelessWidget {
  const ShipmentTimeline({super.key, required this.history});
  final List<ShipmentStatusHistoryEntry> history;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        for (var i = 0; i < history.length; i++) ...[
          _entry(theme, history[i], isCurrent: i == history.length - 1),
          if (i != history.length - 1) const SizedBox(height: 8),
        ],
      ],
    );
  }

  Widget _entry(
    ThemeData theme,
    ShipmentStatusHistoryEntry entry, {
    required bool isCurrent,
  }) => Row(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Icon(
        isCurrent ? Icons.radio_button_checked : Icons.circle_outlined,
        size: 14,
        color: isCurrent ? theme.colorScheme.primary : theme.colorScheme.outline,
      ),
      const SizedBox(width: 8),
      Expanded(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              shipmentStatusLabel(entry.status),
              style: isCurrent
                  ? theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    )
                  : theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.outline,
                    ),
            ),
            Text(
              _dateFormat.format(entry.createdAt),
              style: theme.textTheme.bodySmall,
            ),
            if (entry.trackingNumber != null)
              Text(
                'Takip No: ${entry.trackingNumber}',
                style: theme.textTheme.bodySmall,
              ),
          ],
        ),
      ),
    ],
  );
}

/// `OrderDto` kargo firmasının yalnızca id'sini taşıyor, adını taşımıyor.
/// Ad, Checkout feature'ın (aktif firmaları listeleyen) mevcut
/// `shippingCompaniesProvider`'ı üzerinden çözülür — sipariş verildikten
/// sonra firma pasife alınmışsa (artık aktif listede yoksa) genel bir
/// etikete düşülür.
class ShippingCompanyName extends ConsumerWidget {
  const ShippingCompanyName({
    super.key,
    required this.shippingCompanyId,
    this.style,
  });
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
