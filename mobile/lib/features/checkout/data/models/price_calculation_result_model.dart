import '../../domain/entities/price_calculation.dart';

/// `PriceCalculationResultDto` şekline karşılık gelir. Yalnızca ödeme
/// özetinde gösterilen alanlar taşınır (`lines`/`appliedCoupons`/`taxRate`
/// şu an kullanılmıyor).
class PriceCalculationResultModel extends PriceCalculation {
  const PriceCalculationResultModel({
    required super.subtotal,
    required super.totalDiscount,
    required super.taxAmount,
    required super.grandTotal,
  });

  factory PriceCalculationResultModel.fromJson(Map<String, dynamic> json) =>
      PriceCalculationResultModel(
        subtotal: (json['subtotal'] as num).toDouble(),
        totalDiscount: (json['totalDiscount'] as num).toDouble(),
        taxAmount: (json['taxAmount'] as num).toDouble(),
        grandTotal: (json['grandTotal'] as num).toDouble(),
      );
}
