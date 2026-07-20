import 'package:flutter/material.dart';

import '../../../cart/domain/entities/cart_item.dart';

class OrderSummaryView extends StatelessWidget {
  const OrderSummaryView({super.key, required this.items});
  final List<CartItem> items;

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Sipariş Özeti', style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: 12),
          ...items.map(
            (item) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 4),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(
                      '${item.title} x${item.quantity}',
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  Text('\$${item.lineTotal.toStringAsFixed(2)}'),
                ],
              ),
            ),
          ),
        ],
      ),
    ),
  );
}
