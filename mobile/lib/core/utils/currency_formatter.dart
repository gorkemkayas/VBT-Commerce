import 'package:intl/intl.dart';

/// `NumberFormat.currency(locale: 'tr_TR', symbol: '₺')`'ün varsayılan
/// deseni sembolü başa koyar ("₺8.999,00"); `customPattern` ile sonda
/// gösterecek şekilde (`"8.999,00 ₺"`) geçersiz kılınıyor.
final _tryCurrencyFormat = NumberFormat.currency(
  locale: 'tr_TR',
  symbol: '₺',
  customPattern: '#,##0.00 ¤',
);

/// Uygulamadaki tüm fiyat gösterimleri bu extension üzerinden formatlanır
/// (ör. "8.999,00 ₺") — binlik ayracı nokta, ondalık ayracı virgül. Kuruş
/// kısmı her zaman iki hane gösterilir; bazı fiyatlarda küsurat olup
/// bazılarında olmadığında ekranlar arasında tutarsız görünüm (bazen
/// kuruşlu, bazen kuruşsuz) oluşmasın diye.
extension PriceFormatting on double {
  String toTryCurrency() => _tryCurrencyFormat.format(this);
}
