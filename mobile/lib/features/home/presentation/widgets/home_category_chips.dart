import 'package:flutter/material.dart';

import '../../../product/domain/entities/category.dart';

/// Yatay kaydırılabilir kategori seçici. `null` değeri "Tümü" anlamına gelir.
/// Chip etiketi kategori adını gösterir; seçim ise kategori id'sini iletir
/// (ürün filtrelemesi `categoryId` üzerinden yapılır).
class HomeCategoryChips extends StatelessWidget {
  const HomeCategoryChips({
    super.key,
    required this.categories,
    required this.selectedCategory,
    required this.onCategorySelected,
  });

  final List<Category> categories;
  final String? selectedCategory;
  final ValueChanged<String?> onCategorySelected;

  @override
  Widget build(BuildContext context) {
    if (categories.isEmpty) return const SizedBox.shrink();

    return SizedBox(
      height: 48,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        itemCount: categories.length + 1,
        separatorBuilder: (context, index) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          if (index == 0) {
            return ChoiceChip(
              label: const Text('Tümü'),
              selected: selectedCategory == null,
              onSelected: (_) => onCategorySelected(null),
            );
          }
          final category = categories[index - 1];
          return ChoiceChip(
            label: Text(category.name),
            selected: selectedCategory == category.id,
            onSelected: (selected) =>
                onCategorySelected(selected ? category.id : null),
          );
        },
      ),
    );
  }
}
