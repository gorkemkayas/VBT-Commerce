import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/cart_item.dart';
import '../repositories/cart_repository.dart';

class AddToCartUseCase {
  const AddToCartUseCase(this._repository);
  final CartRepository _repository;

  Future<Result<List<CartItem>>> call({
    required String sellableItemId,
    required bool isVariant,
    required String title,
    required String imageUrl,
    int quantity = 1,
  }) {
    if (sellableItemId.isEmpty || quantity <= 0) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz sepet ürünü.')),
      );
    }
    return _repository.addToCart(
      sellableItemId: sellableItemId,
      isVariant: isVariant,
      title: title,
      imageUrl: imageUrl,
      quantity: quantity,
    );
  }
}
