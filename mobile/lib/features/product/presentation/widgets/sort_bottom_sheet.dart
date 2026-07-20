import 'package:flutter/material.dart';

import '../../domain/entities/product_filter.dart';

class SortBottomSheet extends StatelessWidget {
  const SortBottomSheet({
    super.key,
    required this.selected,
    required this.onSelected,
  });
  final ProductSortOption selected;
  final ValueChanged<ProductSortOption> onSelected;

  static Future<void> show(
    BuildContext context, {
    required ProductSortOption selected,
    required ValueChanged<ProductSortOption> onSelected,
  }) => showModalBottomSheet<void>(
    context: context,
    builder: (_) => SortBottomSheet(selected: selected, onSelected: onSelected),
  );

  @override
  Widget build(BuildContext context) => SafeArea(
    child: RadioGroup<ProductSortOption>(
      groupValue: selected,
      onChanged: (value) {
        if (value == null) return;
        onSelected(value);
        Navigator.of(context).pop();
      },
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const ListTile(title: Text('Sırala')),
          for (final option in ProductSortOption.values)
            RadioListTile<ProductSortOption>(
              value: option,
              title: Text(_label(option)),
            ),
        ],
      ),
    ),
  );

  String _label(ProductSortOption option) => switch (option) {
    ProductSortOption.none => 'Varsayılan',
    ProductSortOption.priceAscending => 'Fiyat artan',
    ProductSortOption.priceDescending => 'Fiyat azalan',
  };
}
