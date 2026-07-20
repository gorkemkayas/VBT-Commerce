import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/domain/repositories/cart_repository.dart';
import '../entities/address.dart';
import '../entities/order.dart';
import '../repositories/checkout_repository.dart';

class CompleteOrderUseCase {
  const CompleteOrderUseCase(this._checkoutRepository, this._cartRepository);
  final CheckoutRepository _checkoutRepository;
  final CartRepository _cartRepository;

  Future<Result<Order>> call({
    required Address address,
    required List<CartItem> items,
  }) async {
    final addressError = address.validationError;
    if (addressError != null) {
      return Result.failure(ValidationFailure(addressError));
    }
    if (items.isEmpty) {
      return const Result.failure(ValidationFailure('Sepetiniz boş.'));
    }
    final result = await _checkoutRepository.completeOrder(
      address: address,
      items: items,
    );
    if (result case Success<Order>()) {
      await _cartRepository.clearCart();
    }
    return result;
  }
}
