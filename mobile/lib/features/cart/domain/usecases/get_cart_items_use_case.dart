import '../../../../core/utils/result.dart';
import '../entities/cart_item.dart';
import '../repositories/cart_repository.dart';

class GetCartItemsUseCase {
  const GetCartItemsUseCase(this._repository);
  final CartRepository _repository;
  Future<Result<List<CartItem>>> call() => _repository.getCartItems();
}
