import 'package:flutter/material.dart';

class PaymentSummaryView extends StatelessWidget {
  const PaymentSummaryView({super.key, required this.subtotal});
  final double subtotal;

  static const shippingFee = 0.0;

  @override
  Widget build(BuildContext context) {
    final total = subtotal + shippingFee;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Ödeme Özeti', style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 12),
            _SummaryRow(
              label: 'Ara Toplam',
              value: subtotal,
              style: Theme.of(context).textTheme.bodyLarge,
            ),
            _SummaryRow(
              label: 'Kargo',
              value: shippingFee,
              style: Theme.of(context).textTheme.bodyLarge,
            ),
            const Divider(height: 24),
            _SummaryRow(
              label: 'Toplam',
              value: total,
              style: Theme.of(context).textTheme.titleLarge,
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
        Text('\$${value.toStringAsFixed(2)}', style: style),
      ],
    ),
  );
}
