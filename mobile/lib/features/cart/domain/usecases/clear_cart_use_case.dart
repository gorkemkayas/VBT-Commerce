import '../../../../core/utils/result.dart';
import '../entities/cart_item.dart';
import '../repositories/cart_repository.dart';

class ClearCartUseCase {
  const ClearCartUseCase(this._repository);
  final CartRepository _repository;
  Future<Result<List<CartItem>>> call() => _repository.clearCart();
}
