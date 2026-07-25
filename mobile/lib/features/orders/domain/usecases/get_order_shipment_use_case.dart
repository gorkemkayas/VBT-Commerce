import '../../../../core/utils/result.dart';
import '../entities/shipment_tracking.dart';
import '../repositories/order_repository.dart';

class GetOrderShipmentUseCase {
  const GetOrderShipmentUseCase(this._repository);
  final OrderRepository _repository;

  Future<Result<ShipmentTracking?>> call(String orderId) =>
      _repository.getOrderShipment(orderId);
}
