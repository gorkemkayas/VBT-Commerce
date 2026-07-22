import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/price_calculation.dart';
import '../repositories/checkout_repository.dart';

class CalculatePriceUseCase {
  const CalculatePriceUseCase(this._repository);
  final CheckoutRepository _repository;

  /// Backend `Items` alanını boş kabul etmiyor; sepet boşken ağa hiç
  /// gitmeden yerel sıfır sonucu döner.
  Future<Result<PriceCalculation>> call(List<CartItem> items) {
    if (items.isEmpty) {
      return Future.value(
        const Result.success(
          PriceCalculation(
            subtotal: 0,
            totalDiscount: 0,
            taxAmount: 0,
            grandTotal: 0,
          ),
        ),
      );
    }
    return _repository.calculatePrice(items);
  }
}
