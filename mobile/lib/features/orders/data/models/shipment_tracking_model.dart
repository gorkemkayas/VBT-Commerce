import '../../domain/entities/shipment_status_history_entry.dart';
import '../../domain/entities/shipment_tracking.dart';

/// `GET /api/orders/me/{orderId}/shipment` yanıtındaki `ShipmentTrackingDto`'ya
/// karşılık gelir.
class ShipmentTrackingModel extends ShipmentTracking {
  const ShipmentTrackingModel({
    required super.id,
    required super.status,
    required super.createdAt,
    super.trackingNumber,
    super.updatedAt,
    super.history,
  });

  factory ShipmentTrackingModel.fromJson(Map<String, dynamic> json) =>
      ShipmentTrackingModel(
        id: json['id'] as String? ?? '',
        status: json['status'] as String? ?? '',
        trackingNumber: json['trackingNumber'] as String?,
        createdAt:
            DateTime.tryParse(json['createdAt'] as String? ?? '')?.toLocal() ??
            DateTime.now(),
        updatedAt: (json['updatedAt'] as String?) == null
            ? null
            : DateTime.tryParse(json['updatedAt'] as String)?.toLocal(),
        history: _historyFromJson(json['history']),
      );
}

List<ShipmentStatusHistoryEntry> _historyFromJson(Object? json) {
  if (json is! List) return const [];
  return json
      .whereType<Map<String, dynamic>>()
      .map(
        (item) => ShipmentStatusHistoryEntry(
          status: item['status'] as String? ?? '',
          trackingNumber: item['trackingNumber'] as String?,
          createdAt:
              DateTime.tryParse(item['createdAt'] as String? ?? '')
                  ?.toLocal() ??
              DateTime.now(),
        ),
      )
      .toList(growable: false);
}
