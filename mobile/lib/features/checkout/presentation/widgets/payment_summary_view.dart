import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/price_calculation.dart';
import '../providers/checkout_providers.dart';

/// Backend'in gerçek fiyat hesaplamasını (`POST /api/pricing/calculate/me`
/// — vergi/indirim dahil) gösterir; sepetin yerel toplamını kullanmaz.
class PaymentSummaryView extends ConsumerWidget {
  const PaymentSummaryView({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final calculation = ref.watch(priceCalculationProvider);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: calculation.when(
          data: (result) => switch (result) {
            Success<PriceCalculation>(:final value) => _SummaryColumn(
              calculation: value,
            ),
            ResultFailure<PriceCalculation>(:final failure) => _MessageColumn(
              message: failure.message,
            ),
          },
          loading: () => const _LoadingRow(),
          error: (_, _) => const _MessageColumn(message: 'Fiyat hesaplanamadı.'),
        ),
      ),
    );
  }
}

class _SummaryColumn extends StatelessWidget {
  const _SummaryColumn({required this.calculation});
  final PriceCalculation calculation;

  @override
  Widget build(BuildContext context) {
    final bodyStyle = Theme.of(context).textTheme.bodyLarge;
    final totalStyle = Theme.of(context).textTheme.titleLarge;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Ödeme Özeti', style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 12),
        _SummaryRow(
          label: 'Ara Toplam',
          value: calculation.subtotal,
          style: bodyStyle,
        ),
        if (calculation.totalDiscount > 0)
          _SummaryRow(
            label: 'İndirim',
            value: -calculation.totalDiscount,
            style: bodyStyle,
          ),
        _SummaryRow(
          label: 'Vergi',
          value: calculation.taxAmount,
          style: bodyStyle,
        ),
        const Divider(height: 24),
        _SummaryRow(
          label: 'Toplam',
          value: calculation.grandTotal,
          style: totalStyle,
        ),
      ],
    );
  }
}

class _LoadingRow extends StatelessWidget {
  const _LoadingRow();

  @override
  Widget build(BuildContext context) => Row(
    children: [
      Text('Ödeme Özeti', style: Theme.of(context).textTheme.titleMedium),
      const SizedBox(width: 12),
      const SizedBox(
        height: 14,
        width: 14,
        child: CircularProgressIndicator(strokeWidth: 2),
      ),
    ],
  );
}

class _MessageColumn extends StatelessWidget {
  const _MessageColumn({required this.message});
  final String message;

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text('Ödeme Özeti', style: Theme.of(context).textTheme.titleMedium),
      const SizedBox(height: 8),
      Text(message),
    ],
  );
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
