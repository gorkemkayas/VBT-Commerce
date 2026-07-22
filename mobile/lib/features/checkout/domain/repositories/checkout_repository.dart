import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/order.dart';
import '../entities/price_calculation.dart';

abstract interface class CheckoutRepository {
  Future<Result<Order>> completeOrder({
    required String addressId,
    required List<CartItem> items,
  });

  /// `POST /api/pricing/calculate/me` üzerinden vergi/indirim dahil gerçek
  /// sipariş toplamını hesaplar (kupon kodu girişi henüz yok, her zaman boş
  /// liste gönderilir).
  Future<Result<PriceCalculation>> calculatePrice(List<CartItem> items);
}
