import '../../../../core/utils/result.dart';
import '../entities/order.dart';
import '../repositories/order_repository.dart';

class GetOrderByIdUseCase {
  const GetOrderByIdUseCase(this._repository);
  final OrderRepository _repository;

  Future<Result<Order>> call(String orderId) => _repository.getOrderById(orderId);
}
