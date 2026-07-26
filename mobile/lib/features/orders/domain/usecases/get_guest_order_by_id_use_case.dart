import '../../../../core/utils/result.dart';
import '../entities/order.dart';
import '../repositories/order_repository.dart';

class GetGuestOrderByIdUseCase {
  const GetGuestOrderByIdUseCase(this._repository);
  final OrderRepository _repository;

  Future<Result<Order>> call({
    required String guestCustomerId,
    required String orderId,
  }) => _repository.getGuestOrderById(
    guestCustomerId: guestCustomerId,
    orderId: orderId,
  );
}
