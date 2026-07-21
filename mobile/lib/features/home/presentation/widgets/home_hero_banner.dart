import 'package:flutter/material.dart';

class HomeHeroBanner extends StatelessWidget {
  const HomeHeroBanner({
    super.key,
    required this.onActionTap,
    this.eyebrow = 'YENİ SEZON',
    this.title = 'Sezonun\nöne çıkanları',
    this.actionLabel = 'Koleksiyonu keşfet',
  });

  final VoidCallback onActionTap;
  final String eyebrow;
  final String title;
  final String actionLabel;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = theme.colorScheme;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(12),
        child: Container(
          height: 260,
          width: double.infinity,
          color: colors.surfaceContainerHighest,
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.end,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                eyebrow,
                style: theme.textTheme.labelMedium?.copyWith(
                  letterSpacing: 1.6,
                  color: colors.onSurfaceVariant,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                title,
                style: theme.textTheme.headlineMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                  height: 1.15,
                ),
              ),
              const SizedBox(height: 20),
              FilledButton(
                onPressed: onActionTap,
                child: Text(actionLabel),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
