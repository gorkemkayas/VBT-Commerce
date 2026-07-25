import 'shipment_status_history_entry.dart';

class ShipmentTracking {
  const ShipmentTracking({
    required this.id,
    required this.status,
    required this.createdAt,
    this.trackingNumber,
    this.updatedAt,
    this.history = const [],
  });

  final String id;
  final String status;
  final String? trackingNumber;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final List<ShipmentStatusHistoryEntry> history;
}
