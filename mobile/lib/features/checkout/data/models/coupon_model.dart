import '../../domain/entities/coupon.dart';

/// `GET /api/coupons/active` yanıtındaki `CouponDto` şekline karşılık gelir.
/// DTO'nun kullanım limiti / kapsam (`scopeType`, `totalUsageLimit` vb.)
/// alanları okunmuyor: uç zaten yalnızca aktif kuponları döner ve kuponun
/// gerçekten uygulanabilir olup olmadığına backend, fiyat hesaplama
/// sırasında karar verir (bkz. `CheckoutController.applyCoupon`).
class CouponModel extends Coupon {
  const CouponModel({
    required super.id,
    required super.code,
    required super.discountType,
    required super.discountValue,
    super.minCartAmount,
    super.maxDiscountAmount,
    super.endDate,
  });

  factory CouponModel.fromJson(Map<String, dynamic> json) => CouponModel(
    id: json['id'] as String,
    code: json['code'] as String,
    discountType: _discountTypeFromJson(json['discountType']),
    discountValue: (json['discountValue'] as num?)?.toDouble() ?? 0,
    minCartAmount: (json['minCartAmount'] as num?)?.toDouble(),
    maxDiscountAmount: (json['maxDiscountAmount'] as num?)?.toDouble(),
    endDate: DateTime.tryParse(json['endDate'] as String? ?? ''),
  );
}

/// Enum'lar yanıtlarda string gelir ("Percentage"/"FixedAmount"); yine de
/// sayısal gösterim (`0`/`1`) ihtimaline karşı ikisi de karşılanır.
CouponDiscountType _discountTypeFromJson(Object? value) => switch (value) {
  'Percentage' || 0 => CouponDiscountType.percentage,
  'FixedAmount' || 1 => CouponDiscountType.fixedAmount,
  _ => CouponDiscountType.unknown,
};
