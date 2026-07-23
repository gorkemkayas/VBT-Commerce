import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../../cart/domain/repositories/cart_repository.dart';
import '../entities/guest_checkout_info.dart';
import '../entities/order.dart';
import '../repositories/checkout_repository.dart';

class CompleteGuestOrderUseCase {
  const CompleteGuestOrderUseCase(this._checkoutRepository, this._cartRepository);
  final CheckoutRepository _checkoutRepository;
  final CartRepository _cartRepository;

  Future<Result<Order>> call({
    required String guestCustomerId,
    required String anonymousId,
    required String? shippingCompanyId,
    required GuestCheckoutInfo info,
    required List<CartItem> items,
  }) async {
    if (shippingCompanyId == null || shippingCompanyId.isEmpty) {
      return const Result.failure(
        ValidationFailure('Lütfen bir kargo firması seçin.'),
      );
    }
    if (items.isEmpty) {
      return const Result.failure(ValidationFailure('Sepetiniz boş.'));
    }
    final result = await _checkoutRepository.completeGuestOrder(
      guestCustomerId: guestCustomerId,
      anonymousId: anonymousId,
      shippingCompanyId: shippingCompanyId,
      info: info,
      items: items,
    );
    if (result case Success<Order>()) {
      await _cartRepository.clearCart();
    }
    return result;
  }
}
