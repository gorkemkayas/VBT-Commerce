class ShipmentStatusHistoryEntry {
  const ShipmentStatusHistoryEntry({
    required this.status,
    required this.createdAt,
    this.trackingNumber,
  });

  /// Backend'in `ShipmentStatus` değeri: `Pending` | `Shipped` | `InTransit`
  /// | `Delivered` | `Cancelled`. Türkçe karşılığı sunum katmanında çözülür
  /// (`Order.status` ile aynı konvansiyon — Dart enum değil).
  final String status;
  final DateTime createdAt;
  final String? trackingNumber;
}
