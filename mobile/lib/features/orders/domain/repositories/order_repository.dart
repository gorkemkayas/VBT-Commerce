import '../../../../core/utils/result.dart';
import '../entities/order.dart';
import '../entities/shipment_tracking.dart';

abstract interface class OrderRepository {
  Future<Result<List<Order>>> getMyOrders();
  Future<Result<Order>> getOrderById(String orderId);
  Future<Result<bool>> cancelOrder(String orderId);

  /// `GET /api/orders/me/{orderId}/shipment`. Sipariş henüz kargoya
  /// verilmediyse (backend `OrderShipmentNotFoundException` ile 404 döner)
  /// bu bir hata değildir — `null` döner.
  Future<Result<ShipmentTracking?>> getOrderShipment(String orderId);
}
