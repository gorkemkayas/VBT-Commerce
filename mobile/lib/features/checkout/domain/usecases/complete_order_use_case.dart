import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/domain/repositories/cart_repository.dart';
import '../entities/order.dart';
import '../repositories/checkout_repository.dart';

class CompleteOrderUseCase {
  const CompleteOrderUseCase(this._checkoutRepository, this._cartRepository);
  final CheckoutRepository _checkoutRepository;
  final CartRepository _cartRepository;

  Future<Result<Order>> call({
    required String? addressId,
    required String? shippingCompanyId,
    required List<CartItem> items,
  }) async {
    if (addressId == null || addressId.isEmpty) {
      return const Result.failure(
        ValidationFailure('Lütfen bir teslimat adresi seçin.'),
      );
    }
    if (shippingCompanyId == null || shippingCompanyId.isEmpty) {
      return const Result.failure(
        ValidationFailure('Lütfen bir kargo firması seçin.'),
      );
    }
    if (items.isEmpty) {
      return const Result.failure(ValidationFailure('Sepetiniz boş.'));
    }
    final result = await _checkoutRepository.completeOrder(
      addressId: addressId,
      shippingCompanyId: shippingCompanyId,
      items: items,
    );
    if (result case Success<Order>()) {
      await _cartRepository.clearCart();
    }
    return result;
  }
}
