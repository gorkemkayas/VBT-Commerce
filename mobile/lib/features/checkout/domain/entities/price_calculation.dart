/// `POST /api/pricing/calculate/me` yanıtındaki (`PriceCalculationResultDto`)
/// ödeme özetinde gösterilen alt kümesi.
class PriceCalculation {
  const PriceCalculation({
    required this.subtotal,
    required this.totalDiscount,
    required this.taxAmount,
    required this.grandTotal,
    this.appliedCoupons = const [],
  });

  final double subtotal;
  final double totalDiscount;
  final double taxAmount;
  final double grandTotal;

  /// Backend'in geçerli sayıp uyguladığı kuponlar (kod + o kupondan gelen
  /// indirim). Kullanıcının girdiği ama geçersiz olan kodlar burada yer almaz;
  /// onlar hesaplama isteğini hata ile döndürür (bkz. `CheckoutController`).
  final List<AppliedCoupon> appliedCoupons;
}

/// Uygulanan tek bir kupon: backend'deki kanonik kodu ve sağladığı indirim.
class AppliedCoupon {
  const AppliedCoupon({required this.code, required this.discountAmount});

  final String code;
  final double discountAmount;
}
