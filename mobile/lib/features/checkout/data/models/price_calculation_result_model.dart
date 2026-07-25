import '../../domain/entities/price_calculation.dart';

/// `PriceCalculationResultDto` şekline karşılık gelir. Ödeme özetinde
/// gösterilen alanlar taşınır; `appliedCoupons` kupon indirimlerini kod
/// bazında göstermek için okunur (`lines`/`taxRate` şu an kullanılmıyor).
class PriceCalculationResultModel extends PriceCalculation {
  const PriceCalculationResultModel({
    required super.subtotal,
    required super.totalDiscount,
    required super.taxAmount,
    required super.grandTotal,
    super.appliedCoupons,
  });

  factory PriceCalculationResultModel.fromJson(Map<String, dynamic> json) =>
      PriceCalculationResultModel(
        subtotal: (json['subtotal'] as num).toDouble(),
        totalDiscount: (json['totalDiscount'] as num).toDouble(),
        taxAmount: (json['taxAmount'] as num).toDouble(),
        grandTotal: (json['grandTotal'] as num).toDouble(),
        appliedCoupons: _couponsFromJson(json['appliedCoupons']),
      );

  static List<AppliedCoupon> _couponsFromJson(Object? json) {
    if (json is! List) return const [];
    return json
        .whereType<Map<String, dynamic>>()
        .map(
          (item) => AppliedCoupon(
            code: item['code'] as String? ?? '',
            discountAmount: (item['discountAmount'] as num?)?.toDouble() ?? 0,
          ),
        )
        .toList(growable: false);
  }
}
