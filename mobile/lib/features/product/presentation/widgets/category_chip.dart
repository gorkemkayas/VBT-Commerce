import 'package:flutter/material.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_motion.dart';

class CategoryChip extends StatelessWidget {
  const CategoryChip({
    super.key,
    required this.label,
    required this.selected,
    required this.onSelected,
  });
  final String label;
  final bool selected;
  final ValueChanged<bool> onSelected;

  @override
  Widget build(BuildContext context) {
    final labelStyle =
        Theme.of(context).chipTheme.labelStyle ?? const TextStyle();

    return Padding(
      padding: const EdgeInsets.only(right: 8),
      child: FilterChip(
        // Etiket rengi elle animasyonlanıyor: FilterChip arka planını yumuşak
        // geçirir ama etiket stilini anlık değiştirir. Aradaki fark yüzünden
        // seçim sırasında beyaz metin bir an açık zeminde kalıyordu.
        label: AnimatedDefaultTextStyle(
          duration: AppMotion.selection,
          curve: Curves.easeOut,
          style: labelStyle.copyWith(
            color: selected
                ? AppColors.onSelectedSurface
                : AppColors.onSurface,
          ),
          overflow: TextOverflow.fade,
          maxLines: 1,
          softWrap: false,
          child: Text(label),
        ),
        selected: selected,
        onSelected: onSelected,
      ),
    );
  }
}
