class Order {
  const Order({
    required this.id,
    required this.status,
    required this.createdAt,
    required this.totalAmount,
    this.itemCount = 0,
  });

  final String id;

  /// Backend'in `OrderStatus` değeri: `Pending` | `Confirmed` | `Cancelled`.
  /// Kullanıcıya gösterilecek Türkçe karşılığı sunum katmanında çözülür.
  final String status;
  final DateTime createdAt;

  /// Siparişin ödenen toplamı (`grandTotal`) — vergi ve kargo dahil.
  final double totalAmount;
  final int itemCount;
}
