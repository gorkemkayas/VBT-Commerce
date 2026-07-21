/// Tasarımdaki boşluk ve köşe yarıçapı ölçeği (Stitch token'ları).
///
/// Widget'larda çıplak sayı (`EdgeInsets.all(16)`) yerine bu sabitleri
/// kullanın; ölçek değişirse tek yerden güncellenir.
abstract final class AppSpacing {
  static const double xs = 4;
  static const double sm = 8;
  static const double md = 16;
  static const double lg = 24;
  static const double xl = 32;
  static const double xxl = 48;

  /// Ekran kenar boşluğu. Tasarımda 20, mevcut sayfalarda 16 kullanılıyor —
  /// kademeli geçiş için 16'da bırakıldı.
  static const double screenPadding = md;
}

/// Tasarım köşeleri belirgin şekilde keskin: varsayılan 2px.
abstract final class AppRadius {
  static const double sm = 2;
  static const double md = 4;
  static const double lg = 8;
  static const double xl = 12;

  /// Hap şeklindeki chip'ler için.
  static const double pill = 999;
}
