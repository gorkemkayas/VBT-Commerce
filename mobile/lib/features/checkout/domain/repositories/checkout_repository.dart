import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/order.dart';
import '../entities/price_calculation.dart';
import '../entities/shipping_company.dart';

abstract interface class CheckoutRepository {
  Future<Result<Order>> completeOrder({
    required String addressId,
    required String shippingCompanyId,
    required List<CartItem> items,
  });

  /// `POST /api/pricing/calculate/me` üzerinden vergi/indirim dahil gerçek
  /// sipariş toplamını hesaplar (kupon kodu girişi henüz yok, her zaman boş
  /// liste gönderilir).
  Future<Result<PriceCalculation>> calculatePrice(List<CartItem> items);

  /// `GET /api/shipping-companies` — aktif kargo firmalarının tamamını döner.
  Future<Result<List<ShippingCompany>>> getShippingCompanies();
}
