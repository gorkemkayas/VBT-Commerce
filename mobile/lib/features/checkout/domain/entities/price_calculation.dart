/// `POST /api/pricing/calculate/me` yanıtındaki (`PriceCalculationResultDto`)
/// ödeme özetinde gösterilen alt kümesi.
class PriceCalculation {
  const PriceCalculation({
    required this.subtotal,
    required this.totalDiscount,
    required this.taxAmount,
    required this.grandTotal,
  });

  final double subtotal;
  final double totalDiscount;
  final double taxAmount;
  final double grandTotal;
}
