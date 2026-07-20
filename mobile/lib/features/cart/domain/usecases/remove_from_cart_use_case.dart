import '../../../../core/utils/result.dart';
import '../entities/cart_item.dart';
import '../repositories/cart_repository.dart';

class RemoveFromCartUseCase {
  const RemoveFromCartUseCase(this._repository);
  final CartRepository _repository;
  Future<Result<List<CartItem>>> call(String itemId) =>
      _repository.removeFromCart(itemId);
}
