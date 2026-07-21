import 'package:flutter/material.dart';

import 'app_colors.dart';
import 'app_spacing.dart';
import 'app_typography.dart';

/// Uygulama teması. Renk ve tipografi token'ları `app_colors.dart` ve
/// `app_typography.dart` içinde tanımlıdır; burada yalnızca bileşen
/// temaları birleştirilir.
abstract final class AppTheme {
  static ThemeData get light {
    const colors = AppColors.lightScheme;

    return ThemeData(
      useMaterial3: true,
      colorScheme: colors,
      scaffoldBackgroundColor: colors.surface,
      textTheme: AppTypography.textTheme,
      fontFamily: AppTypography.fontFamily,

      appBarTheme: const AppBarTheme(
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.onSurface,
        surfaceTintColor: Colors.transparent,
        elevation: 0,
        scrolledUnderElevation: 0,
        centerTitle: false,
        titleTextStyle: TextStyle(
          fontFamily: AppTypography.fontFamily,
          fontSize: 20,
          fontWeight: FontWeight.w600,
          letterSpacing: -0.2,
          color: AppColors.onSurface,
        ),
      ),

      // Tasarımda birincil CTA mavi değil, neredeyse siyah.
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          backgroundColor: AppColors.actionSurface,
          foregroundColor: AppColors.onActionSurface,
          minimumSize: const Size.fromHeight(52),
          shape: const RoundedRectangleBorder(
            borderRadius: BorderRadius.all(Radius.circular(AppRadius.sm)),
          ),
          textStyle: AppTypography.textTheme.labelLarge,
        ),
      ),

      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: AppColors.onSurface,
          minimumSize: const Size.fromHeight(52),
          side: const BorderSide(color: AppColors.outlineVariant),
          shape: const RoundedRectangleBorder(
            borderRadius: BorderRadius.all(Radius.circular(AppRadius.sm)),
          ),
          textStyle: AppTypography.textTheme.labelLarge,
        ),
      ),

      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.primary,
          textStyle: AppTypography.textTheme.labelMedium,
        ),
      ),

      inputDecorationTheme: const InputDecorationTheme(
        filled: true,
        fillColor: AppColors.surfaceContainerLowest,
        contentPadding: EdgeInsets.symmetric(
          horizontal: AppSpacing.md,
          vertical: AppSpacing.md,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppRadius.sm)),
          borderSide: BorderSide(color: AppColors.outlineVariant),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppRadius.sm)),
          borderSide: BorderSide(color: AppColors.outlineVariant),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppRadius.sm)),
          borderSide: BorderSide(color: AppColors.primaryContainer, width: 2),
        ),
      ),

      chipTheme: ChipThemeData(
        backgroundColor: AppColors.surfaceContainerHighest,
        selectedColor: AppColors.primaryContainer,
        side: BorderSide.none,
        shape: const StadiumBorder(),
        labelStyle: AppTypography.textTheme.labelMedium,
        secondaryLabelStyle: AppTypography.textTheme.labelMedium?.copyWith(
          color: AppColors.onPrimary,
        ),
        showCheckmark: false,
      ),

      // NOT: Flutter 3.32+ sürümlerinde bu alan `CardThemeData` bekler.
      // Daha eski bir sürüme dönerseniz `CardTheme` olarak değiştirin.
      cardTheme: const CardThemeData(
        color: AppColors.surfaceContainerLowest,
        surfaceTintColor: Colors.transparent,
        elevation: 0,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppRadius.md)),
          side: BorderSide(color: AppColors.outlineVariant),
        ),
      ),

      dividerTheme: const DividerThemeData(
        color: AppColors.outlineVariant,
        thickness: 1,
        space: 1,
      ),

      navigationBarTheme: NavigationBarThemeData(
        backgroundColor: AppColors.surface,
        surfaceTintColor: Colors.transparent,
        indicatorColor: AppColors.onPrimaryContainer,
        elevation: 0,
        labelTextStyle: WidgetStateProperty.resolveWith(
          (states) => states.contains(WidgetState.selected)
              ? AppTypography.textTheme.labelSmall?.copyWith(
                  color: AppColors.primary,
                  fontWeight: FontWeight.w600,
                )
              : AppTypography.textTheme.labelSmall,
        ),
      ),
    );
  }
}
