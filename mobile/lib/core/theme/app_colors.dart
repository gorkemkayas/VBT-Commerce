import 'package:flutter/material.dart';

/// Tasarım sisteminin ham renk token'ları.
///
/// Kaynak: Google Stitch tasarım export'undaki Material 3 token seti.
/// Bu değerler tek doğruluk kaynağıdır — widget'larda hex kodu yazmayın,
/// `Theme.of(context).colorScheme` üzerinden okuyun.
abstract final class AppColors {
  // Primary
  static const primary = Color(0xFF0041C8);
  static const onPrimary = Color(0xFFFFFFFF);
  static const primaryContainer = Color(0xFF0055FF);
  static const onPrimaryContainer = Color(0xFFE3E6FF);

  // Secondary
  static const secondary = Color(0xFF445AA7);
  static const onSecondary = Color(0xFFFFFFFF);
  static const secondaryContainer = Color(0xFF95AAFD);
  static const onSecondaryContainer = Color(0xFF243C88);

  // Tertiary
  static const tertiary = Color(0xFF972500);
  static const onTertiary = Color(0xFFFFFFFF);
  static const tertiaryContainer = Color(0xFFC13301);
  static const onTertiaryContainer = Color(0xFFFFE1D9);

  // Error
  static const error = Color(0xFFBA1A1A);
  static const onError = Color(0xFFFFFFFF);
  static const errorContainer = Color(0xFFFFDAD6);
  static const onErrorContainer = Color(0xFF93000A);

  // Surface
  static const surface = Color(0xFFFCF9F8);
  static const onSurface = Color(0xFF1C1B1B);
  static const onSurfaceVariant = Color(0xFF434656);
  static const surfaceContainerLowest = Color(0xFFFFFFFF);
  static const surfaceContainerLow = Color(0xFFF6F3F2);
  static const surfaceContainer = Color(0xFFF0EDED);
  static const surfaceContainerHigh = Color(0xFFEAE7E7);
  static const surfaceContainerHighest = Color(0xFFE5E2E1);
  static const surfaceDim = Color(0xFFDCD9D9);
  static const surfaceBright = Color(0xFFFCF9F8);

  // Outline
  static const outline = Color(0xFF737688);
  static const outlineVariant = Color(0xFFC3C5D9);

  // Inverse
  static const inverseSurface = Color(0xFF313030);
  static const onInverseSurface = Color(0xFFF3F0EF);
  static const inversePrimary = Color(0xFFB6C4FF);
  static const surfaceTint = Color(0xFF004DEA);

  /// Tasarımdaki birincil aksiyon butonları markanın mavisini değil,
  /// neredeyse siyah olan `onSurface` tonunu kullanır.
  static const actionSurface = onSurface;
  static const onActionSurface = Color(0xFFFFFFFF);

  static const ColorScheme lightScheme = ColorScheme(
    brightness: Brightness.light,
    primary: primary,
    onPrimary: onPrimary,
    primaryContainer: primaryContainer,
    onPrimaryContainer: onPrimaryContainer,
    secondary: secondary,
    onSecondary: onSecondary,
    secondaryContainer: secondaryContainer,
    onSecondaryContainer: onSecondaryContainer,
    tertiary: tertiary,
    onTertiary: onTertiary,
    tertiaryContainer: tertiaryContainer,
    onTertiaryContainer: onTertiaryContainer,
    error: error,
    onError: onError,
    errorContainer: errorContainer,
    onErrorContainer: onErrorContainer,
    surface: surface,
    onSurface: onSurface,
    onSurfaceVariant: onSurfaceVariant,
    surfaceContainerLowest: surfaceContainerLowest,
    surfaceContainerLow: surfaceContainerLow,
    surfaceContainer: surfaceContainer,
    surfaceContainerHigh: surfaceContainerHigh,
    surfaceContainerHighest: surfaceContainerHighest,
    surfaceDim: surfaceDim,
    surfaceBright: surfaceBright,
    outline: outline,
    outlineVariant: outlineVariant,
    inverseSurface: inverseSurface,
    onInverseSurface: onInverseSurface,
    inversePrimary: inversePrimary,
    surfaceTint: surfaceTint,
  );
}
