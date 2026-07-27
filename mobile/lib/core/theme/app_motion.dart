/// Hareket (animasyon) token'ları.
///
/// Renk gibi süreler de tek bir kaynaktan gelir; widget'larda serbest
/// `Duration` yazmayın.
abstract final class AppMotion {
  /// Seçim durumu değişimlerinin süresi (çipler, beden kutuları, alt menü
  /// göstergesi). Material'in kendi chip seçim animasyonuyla (~195ms) aynı
  /// hizada tutuluyor ki elle animasyonlanan bileşenler yerleşik olanlarla
  /// aynı ritimde çalışsın.
  static const selection = Duration(milliseconds: 200);
}
