import '../../../../core/utils/result.dart';
import '../entities/order.dart';

abstract interface class OrderRepository {
  Future<Result<List<Order>>> getMyOrders();
  Future<Result<Order>> getOrderById(String orderId);
  Future<Result<bool>> cancelOrder(String orderId);
}
