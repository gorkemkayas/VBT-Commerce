import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/guest_checkout_info.dart';
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

  /// `POST /api/guest-customers` — misafir checkout'un ilk adımı; dönen id
  /// diğer misafir çağrılarında kullanılır.
  Future<Result<String>> createGuestCustomer({
    required String firstName,
    required String lastName,
    required String email,
    required String phoneNumber,
  });

  /// `POST /api/pricing/calculate/guest`.
  Future<Result<PriceCalculation>> calculatePriceGuest(
    String guestCustomerId,
    List<CartItem> items,
  );

  /// `POST /api/orders/guest`.
  Future<Result<Order>> completeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required GuestCheckoutInfo info,
    required List<CartItem> items,
  });
}
