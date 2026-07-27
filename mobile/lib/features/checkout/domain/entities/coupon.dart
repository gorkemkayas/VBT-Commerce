/// Backend'in `CouponDiscountType` enum'ı. Yanıtlarda string olarak gelir
/// (`JsonStringEnumConverter` kayıtlı, bkz. `ServiceCollectionExtensions`);
/// tanınmayan bir değer gelirse kupon çipinde yalnızca kod gösterilir
/// (bkz. `CouponInput`).
enum CouponDiscountType { percentage, fixedAmount, unknown }

/// `GET /api/coupons/active` ile dönen, herkese açık (vitrin) kupon kaydı.
/// Checkout'taki kupon alanı bunları öneri olarak listeler — kullanıcının
/// kodu bilmeden de indirimden yararlanabilmesi için.
class Coupon {
  const Coupon({
    required this.id,
    required this.code,
    required this.discountType,
    required this.discountValue,
    this.minCartAmount,
    this.maxDiscountAmount,
    this.endDate,
  });

  final String id;
  final String code;
  final CouponDiscountType discountType;
  final double discountValue;

  /// Kuponun geçerli olması için gereken en düşük sepet tutarı; `null` ise
  /// alt sınır yok.
  final double? minCartAmount;

  /// Yüzde indirimlerde uygulanacak üst sınır; `null` ise sınırsız.
  final double? maxDiscountAmount;

  final DateTime? endDate;
}
