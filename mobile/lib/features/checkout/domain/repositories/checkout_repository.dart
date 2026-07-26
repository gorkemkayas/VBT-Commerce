import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/guest_billing_info.dart';
import '../entities/guest_checkout_info.dart';
import '../entities/order.dart';
import '../entities/price_calculation.dart';
import '../entities/shipping_company.dart';

abstract interface class CheckoutRepository {
  /// `billingAddressId`, `null` ise fatura adresi teslimat adresiyle aynı
  /// kabul edilir (backend bu alanı opsiyonel tutar, bkz.
  /// `PlaceMyOrderCommand.BillingAddressId`).
  Future<Result<Order>> completeOrder({
    required String addressId,
    String? billingAddressId,
    required String shippingCompanyId,
    required List<CartItem> items,
    required List<String> couponCodes,
  });

  /// `POST /api/pricing/calculate/me` üzerinden vergi/indirim dahil gerçek
  /// sipariş toplamını hesaplar. `couponCodes`, kullanıcının uyguladığı kupon
  /// kodlarıdır; geçerli olanların indirimi sonuçta döner.
  Future<Result<PriceCalculation>> calculatePrice(
    List<CartItem> items,
    List<String> couponCodes,
  );

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
    List<String> couponCodes,
  );

  /// `POST /api/orders/guest`. `billingInfo`, `null` ise fatura adresi
  /// teslimat adresiyle aynı kabul edilir.
  Future<Result<Order>> completeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required GuestCheckoutInfo info,
    GuestBillingInfo? billingInfo,
    required List<CartItem> items,
    required List<String> couponCodes,
  });
}
