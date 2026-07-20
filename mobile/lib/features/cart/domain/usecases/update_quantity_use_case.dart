import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/cart_item.dart';
import '../repositories/cart_repository.dart';

class UpdateQuantityUseCase {
  const UpdateQuantityUseCase(this._repository);
  final CartRepository _repository;
  Future<Result<List<CartItem>>> call({
    required String itemId,
    required int quantity,
  }) {
    if (itemId.isEmpty || quantity < 0) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz sepet güncellemesi.')),
      );
    }
    return _repository.updateQuantity(itemId: itemId, quantity: quantity);
  }
}
