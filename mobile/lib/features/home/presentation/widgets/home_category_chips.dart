import 'package:flutter/material.dart';

/// Yatay kaydırılabilir kategori seçici. `null` değeri "Tümü" anlamına gelir.
class HomeCategoryChips extends StatelessWidget {
  const HomeCategoryChips({
    super.key,
    required this.categories,
    required this.selectedCategory,
    required this.onCategorySelected,
  });

  final List<String> categories;
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
            label: Text(category),
            selected: selectedCategory == category,
            onSelected: (selected) =>
                onCategorySelected(selected ? category : null),
          );
        },
      ),
    );
  }
}
