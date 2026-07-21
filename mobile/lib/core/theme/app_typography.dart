import 'package:flutter/material.dart';

import 'app_colors.dart';

/// Tasarımdaki tip ölçeği (Stitch token'ları) Material 3 `TextTheme`
/// slotlarına eşlenmiş hâli.
///
/// Not: `letterSpacing` Flutter'da logical pixel cinsindendir; tasarımdaki
/// `em` değerleri font boyutuyla çarpılarak dönüştürülmüştür
/// (ör. 12px üzerinde 0.05em = 0.6px).
abstract final class AppTypography {
  /// Tasarım Inter kullanıyor. Etkinleştirmek için iki seçenek var:
  ///   1. `pubspec.yaml`'a `google_fonts` ekleyip burayı `'Inter'` yapın ve
  ///      `GoogleFonts.interTextTheme(...)` ile sarın, ya da
  ///   2. Inter dosyalarını `fonts/` altına koyup pubspec'te tanımlayın.
  /// `null` bırakılırsa platformun varsayılan fontu kullanılır.
  static const String? fontFamily = null;

  static const TextTheme textTheme = TextTheme(
    displayLarge: TextStyle(
      fontFamily: fontFamily,
      fontSize: 40,
      height: 48 / 40,
      fontWeight: FontWeight.w600,
      letterSpacing: -0.8,
      color: AppColors.onSurface,
    ),
    displayMedium: TextStyle(
      fontFamily: fontFamily,
      fontSize: 36,
      height: 44 / 36,
      fontWeight: FontWeight.w600,
      letterSpacing: -0.72,
      color: AppColors.onSurface,
    ),
    displaySmall: TextStyle(
      fontFamily: fontFamily,
      fontSize: 32,
      height: 40 / 32,
      fontWeight: FontWeight.w600,
      letterSpacing: -0.64,
      color: AppColors.onSurface,
    ),
    headlineLarge: TextStyle(
      fontFamily: fontFamily,
      fontSize: 28,
      height: 34 / 28,
      fontWeight: FontWeight.w600,
      letterSpacing: -0.28,
      color: AppColors.onSurface,
    ),
    headlineMedium: TextStyle(
      fontFamily: fontFamily,
      fontSize: 24,
      height: 30 / 24,
      fontWeight: FontWeight.w600,
      letterSpacing: -0.24,
      color: AppColors.onSurface,
    ),
    headlineSmall: TextStyle(
      fontFamily: fontFamily,
      fontSize: 22,
      height: 28 / 22,
      fontWeight: FontWeight.w600,
      letterSpacing: -0.22,
      color: AppColors.onSurface,
    ),
    titleLarge: TextStyle(
      fontFamily: fontFamily,
      fontSize: 20,
      height: 28 / 20,
      fontWeight: FontWeight.w500,
      letterSpacing: -0.2,
      color: AppColors.onSurface,
    ),
    titleMedium: TextStyle(
      fontFamily: fontFamily,
      fontSize: 16,
      height: 24 / 16,
      fontWeight: FontWeight.w500,
      color: AppColors.onSurface,
    ),
    titleSmall: TextStyle(
      fontFamily: fontFamily,
      fontSize: 14,
      height: 20 / 14,
      fontWeight: FontWeight.w500,
      color: AppColors.onSurface,
    ),
    bodyLarge: TextStyle(
      fontFamily: fontFamily,
      fontSize: 16,
      height: 24 / 16,
      fontWeight: FontWeight.w400,
      color: AppColors.onSurface,
    ),
    bodyMedium: TextStyle(
      fontFamily: fontFamily,
      fontSize: 14,
      height: 20 / 14,
      fontWeight: FontWeight.w400,
      color: AppColors.onSurface,
    ),
    bodySmall: TextStyle(
      fontFamily: fontFamily,
      fontSize: 12,
      height: 16 / 12,
      fontWeight: FontWeight.w400,
      color: AppColors.onSurfaceVariant,
    ),
    labelLarge: TextStyle(
      fontFamily: fontFamily,
      fontSize: 13,
      height: 16 / 13,
      fontWeight: FontWeight.w600,
      letterSpacing: 1.04,
      color: AppColors.onSurface,
    ),
    labelMedium: TextStyle(
      fontFamily: fontFamily,
      fontSize: 12,
      height: 16 / 12,
      fontWeight: FontWeight.w600,
      letterSpacing: 0.6,
      color: AppColors.onSurface,
    ),
    labelSmall: TextStyle(
      fontFamily: fontFamily,
      fontSize: 11,
      height: 14 / 11,
      fontWeight: FontWeight.w500,
      color: AppColors.onSurfaceVariant,
    ),
  );
}
