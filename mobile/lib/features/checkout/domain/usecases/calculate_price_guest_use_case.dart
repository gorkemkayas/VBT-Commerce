import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/price_calculation.dart';
import '../repositories/checkout_repository.dart';

class CalculatePriceGuestUseCase {
  const CalculatePriceGuestUseCase(this._repository);
  final CheckoutRepository _repository;

  Future<Result<PriceCalculation>> call(
    String guestCustomerId,
    List<CartItem> items, [
    List<String> couponCodes = const [],
  ]) => _repository.calculatePriceGuest(guestCustomerId, items, couponCodes);
}
