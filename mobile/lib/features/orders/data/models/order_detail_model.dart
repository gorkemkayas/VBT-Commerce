import '../../domain/entities/order.dart';
import '../../domain/entities/order_address.dart';
import '../../domain/entities/order_item.dart';

/// `GET /api/orders/me/{orderId}` yanıtındaki tam `OrderDto`'ya karşılık
/// gelir — `OrderModel`'in aksine (liste, özet alanlar) burada kalemler,
/// adres ve vergi/indirim kırılımı da doldurulur.
class OrderDetailModel extends Order {
  const OrderDetailModel({
    required super.id,
    required super.status,
    required super.createdAt,
    required super.totalAmount,
    required super.items,
    required super.address,
    required super.subtotal,
    required super.discountAmount,
    required super.taxAmount,
  }) : super(itemCount: items.length);

  factory OrderDetailModel.fromJson(Map<String, dynamic> json) {
    final items = _itemsFromJson(json['items']);
    return OrderDetailModel(
      id: json['id'] as String,
      status: json['status'] as String? ?? '',
      createdAt:
          DateTime.tryParse(json['createdAt'] as String? ?? '')?.toLocal() ??
          DateTime.now(),
      totalAmount: (json['grandTotal'] as num?)?.toDouble() ?? 0,
      items: items,
      address: _addressFromJson(json),
      subtotal: (json['subtotal'] as num?)?.toDouble() ?? 0,
      discountAmount: (json['discountAmount'] as num?)?.toDouble() ?? 0,
      taxAmount: (json['taxAmount'] as num?)?.toDouble() ?? 0,
    );
  }
}

List<OrderItem> _itemsFromJson(Object? json) {
  if (json is! List) return const [];
  return json
      .whereType<Map<String, dynamic>>()
      .map(
        (item) => OrderItem(
          sellableItemId: item['sellableItemId'] as String? ?? '',
          sellableItemType: item['sellableItemType'] as String? ?? '',
          quantity: (item['quantity'] as num?)?.toInt() ?? 0,
          unitPrice: (item['unitPrice'] as num?)?.toDouble() ?? 0,
          lineSubtotal: (item['lineSubtotal'] as num?)?.toDouble() ?? 0,
        ),
      )
      .toList(growable: false);
}

OrderAddress? _addressFromJson(Map<String, dynamic> json) {
  final recipientName = json['recipientName'];
  if (recipientName is! String) return null;
  return OrderAddress(
    recipientName: recipientName,
    phoneNumber: json['phoneNumber'] as String? ?? '',
    country: json['country'] as String? ?? '',
    city: json['city'] as String? ?? '',
    district: json['district'] as String? ?? '',
    postalCode: json['postalCode'] as String? ?? '',
    addressLine1: json['addressLine1'] as String? ?? '',
    addressLine2: json['addressLine2'] as String?,
  );
}
