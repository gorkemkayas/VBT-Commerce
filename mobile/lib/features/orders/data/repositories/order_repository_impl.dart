import '../../../../core/utils/result.dart';
import '../../domain/entities/order.dart';
import '../../domain/repositories/order_repository.dart';

/// Backend hazır olana kadar sipariş listesini boş döner.
class OrderRepositoryImpl implements OrderRepository {
  @override
  Future<Result<List<Order>>> getMyOrders() async {
    return const Result.success(<Order>[]);
  }
}
