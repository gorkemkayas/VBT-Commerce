import 'order_address.dart';
import 'order_item.dart';

class Order {
  const Order({
    required this.id,
    required this.status,
    required this.createdAt,
    required this.totalAmount,
    this.itemCount = 0,
    this.items = const [],
    this.address,
    this.subtotal = 0,
    this.discountAmount = 0,
    this.taxAmount = 0,
    this.shippingCompanyId,
    this.shippingFee = 0,
  });

  final String id;

  /// Backend'in `OrderStatus` değeri: `Pending` | `Confirmed` | `Cancelled`.
  /// Kullanıcıya gösterilecek Türkçe karşılığı sunum katmanında çözülür.
  final String status;
  final DateTime createdAt;

  /// Siparişin ödenen toplamı (`grandTotal`) — vergi ve kargo dahil.
  final double totalAmount;
  final int itemCount;

  /// Sipariş kalemleri. Yalnızca sipariş detayında (`GET /api/orders/me/{id}`)
  /// doldurulur; liste öğelerinde her zaman boştur (bkz. `OrderModel`).
  final List<OrderItem> items;

  /// Teslimat adresi anlık görüntüsü. Yalnızca detayda dolar.
  final OrderAddress? address;

  /// Vergi/indirim öncesi ara toplam. Yalnızca detayda dolar.
  final double subtotal;

  /// Uygulanan kupon indirimi toplamı. Yalnızca detayda dolar.
  final double discountAmount;

  /// Yalnızca detayda dolar.
  final double taxAmount;

  /// Sipariş verilirken seçilen kargo firmasının id'si. Backend bu id'nin
  /// adını taşımıyor — ad, sunum katmanında Checkout feature'ın (aktif
  /// firmaları listeleyen) `shippingCompaniesProvider`'ı üzerinden
  /// çözülür. Yalnızca detayda dolar.
  final String? shippingCompanyId;

  /// Kargo ücreti. `totalAmount` (`grandTotal`) bu tutarı zaten içerir —
  /// burada ayrıca gösterim/kırılım amacıyla tutulur. Yalnızca detayda dolar.
  final double shippingFee;
}
