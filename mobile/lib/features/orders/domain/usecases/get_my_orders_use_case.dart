import '../../../../core/utils/result.dart';
import '../entities/order.dart';
import '../repositories/order_repository.dart';

class GetMyOrdersUseCase {
  const GetMyOrdersUseCase(this._repository);
  final OrderRepository _repository;

  Future<Result<List<Order>>> call() => _repository.getMyOrders();
}
