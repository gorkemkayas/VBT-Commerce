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
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(12),
          boxShadow: [
            BoxShadow(
              color: colors.shadow.withValues(alpha: .12),
              blurRadius: 20,
              offset: const Offset(0, 8),
            ),
          ],
        ),
        child: ClipRRect(
          borderRadius: BorderRadius.circular(12),
          child: Container(
            height: 260,
            width: double.infinity,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: [
                  colors.surfaceContainerHighest,
                  colors.surfaceContainerHigh,
                  colors.surfaceContainerLow,
                ],
              ),
              border: Border.all(
                color: colors.outline.withValues(alpha: .15),
              ),
            ),
            child: Stack(
              children: [
                // Dekoratif arka plan fotoğrafı — ana içerik değil, saf
                // dokusal bir premium doku katmanı; bu yüzden çok düşük
                // opaklıkta. Üzerine gelen gradient, altındaki yazıların
                // her koşulda net okunmasını garanti eder.
                Positioned.fill(
                  child: Opacity(
                    opacity: .22,
                    child: Image.asset(
                      'assets/hero_background.jpg',
                      fit: BoxFit.cover,
                    ),
                  ),
                ),
                Positioned.fill(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.bottomCenter,
                        end: Alignment.topCenter,
                        colors: [
                          colors.surfaceContainerHighest.withValues(
                            alpha: .92,
                          ),
                          colors.surfaceContainerHighest.withValues(
                            alpha: .1,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                // Tek satır marka filigranı — içerik değil, saf dekoratif
                // bir doku öğesi; bu yüzden çok düşük opaklıkta ve mevcut
                // alt-sol içerikle (eyebrow/title/buton) çakışmayacak
                // şekilde üst-sağ boşlukta konumlandırılıyor.
                Positioned(
                  top: 48,
                  right: 24,
                  child: Opacity(
                    opacity: .10,
                    child: Text(
                      'TRENDORA',
                      style: TextStyle(
                        fontSize: 44,
                        fontWeight: FontWeight.w800,
                        letterSpacing: 6,
                        height: .9,
                        color: colors.onSurface,
                      ),
                    ),
                  ),
                ),
                // İnce, destekleyici bir ayraç çizgisi.
                Positioned(
                  top: 28,
                  left: 24,
                  right: 24,
                  child: Container(
                    height: 1,
                    color: colors.outline.withValues(alpha: .15),
                  ),
                ),
                Padding(
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
              ],
            ),
          ),
        ),
      ),
    );
  }
}
