import '../../domain/entities/order.dart';

/// `GET /api/orders/me` yanıtındaki `OrderDto`'ya karşılık gelir. DTO çok daha
/// fazla alan taşır (adres, kupon, vergi kırılımı, kalem detayları); sipariş
/// listesi için yalnızca özet alanlar okunur.
class OrderModel extends Order {
  const OrderModel({
    required super.id,
    required super.status,
    required super.createdAt,
    required super.totalAmount,
    super.itemCount,
  });

  factory OrderModel.fromJson(Map<String, dynamic> json) {
    final items = json['items'];
    return OrderModel(
      id: json['id'] as String,
      // Enum'lar backend'de string olarak serileştirilir (JsonStringEnumConverter).
      status: json['status'] as String? ?? '',
      createdAt:
          DateTime.tryParse(json['createdAt'] as String? ?? '')?.toLocal() ??
          DateTime.now(),
      totalAmount: (json['grandTotal'] as num?)?.toDouble() ?? 0,
      itemCount: items is List ? items.length : 0,
    );
  }
}
