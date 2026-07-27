import 'package:commerce_mobile/core/errors/failure.dart';
import 'package:commerce_mobile/core/utils/result.dart';
import 'package:commerce_mobile/features/cart/domain/entities/cart_item.dart';
import 'package:commerce_mobile/features/cart/domain/repositories/cart_repository.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/coupon.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/guest_billing_info.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/guest_checkout_info.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/guest_contact.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/order.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/payment_card_info.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/price_calculation.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/shipping_company.dart';
import 'package:commerce_mobile/features/checkout/domain/repositories/checkout_repository.dart';
import 'package:commerce_mobile/features/checkout/domain/usecases/complete_order_use_case.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeCheckoutRepository implements CheckoutRepository {
  int callCount = 0;

  @override
  Future<Result<Order>> completeOrder({
    required String addressId,
    String? billingAddressId,
    required String shippingCompanyId,
    required List<CartItem> items,
    required List<String> couponCodes,
    required PaymentCardInfo cardInfo,
  }) async {
    callCount++;
    return Result.success(
      Order(
        orderId: 'TEST-1',
        placedAt: DateTime(2026),
        items: items,
        total: items.fold(0, (total, item) => total + item.lineTotal),
      ),
    );
  }

  @override
  Future<Result<PriceCalculation>> calculatePrice(
    List<CartItem> items,
    List<String> couponCodes,
  ) async {
    return const Result.success(
      PriceCalculation(
        subtotal: 0,
        totalDiscount: 0,
        taxAmount: 0,
        grandTotal: 0,
      ),
    );
  }

  @override
  Future<Result<List<ShippingCompany>>> getShippingCompanies() async {
    return const Result.success([]);
  }

  @override
  Future<Result<String>> createGuestCustomer({
    required String firstName,
    required String lastName,
    required String email,
    required String phoneNumber,
  }) async => const Result.success('guest-customer-1');

  @override
  Future<Result<List<Coupon>>> getActiveCoupons() async =>
      const Result.success([]);

  @override
  Future<Result<GuestContact>> getGuestCustomer(String guestCustomerId) async =>
      const Result.success(
        GuestContact(
          id: 'guest-customer-1',
          firstName: 'Test',
          lastName: 'User',
          email: 'test@example.com',
          phoneNumber: '5550000000',
        ),
      );

  @override
  Future<Result<PriceCalculation>> calculatePriceGuest(
    String guestCustomerId,
    List<CartItem> items,
    List<String> couponCodes,
  ) async => const Result.success(
    PriceCalculation(subtotal: 0, totalDiscount: 0, taxAmount: 0, grandTotal: 0),
  );

  @override
  Future<Result<Order>> completeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required GuestCheckoutInfo info,
    GuestBillingInfo? billingInfo,
    required List<CartItem> items,
    required List<String> couponCodes,
    required PaymentCardInfo cardInfo,
  }) async => Result.success(
    Order(
      orderId: 'TEST-GUEST-1',
      placedAt: DateTime(2026),
      items: items,
      total: items.fold(0, (total, item) => total + item.lineTotal),
    ),
  );
}

class _FakeCartRepository implements CartRepository {
  bool cleared = false;

  @override
  Future<Result<List<CartItem>>> addToCart({
    required String sellableItemId,
    required bool isVariant,
    required String title,
    required String imageUrl,
    String? variantLabel,
    int quantity = 1,
  }) async => const Result.success([]);

  @override
  Future<Result<List<CartItem>>> clearCart() async {
    cleared = true;
    return const Result.success([]);
  }

  @override
  Future<Result<List<CartItem>>> getCartItems() async =>
      const Result.success([]);

  @override
  Future<Result<List<CartItem>>> removeFromCart(String itemId) async =>
      const Result.success([]);

  @override
  Future<Result<List<CartItem>>> updateQuantity({
    required String itemId,
    required int quantity,
  }) async => const Result.success([]);
}

void main() {
  const cardInfo = PaymentCardInfo(
    cardHolderName: 'Test User',
    cardNumber: '5528790000000008',
    cardExpireMonth: '12',
    cardExpireYear: '2030',
    cardCvc: '123',
    buyerIdentityNumber: '74300864791',
  );
  const item = CartItem(
    id: 'item-1',
    sellableItemId: 'variant-1',
    sellableItemType: 'Variant',
    title: 'Ürün',
    unitPrice: 100,
    imageUrl: '',
    quantity: 2,
  );

  test(
    'adres seçilmemişse ValidationFailure döner ve repository çağrılmaz',
    () async {
      final checkoutRepository = _FakeCheckoutRepository();
      final cartRepository = _FakeCartRepository();
      final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

      final result = await useCase(
        addressId: null,
        shippingCompanyId: 'shipping-1',
        items: [item],
        cardInfo: cardInfo,
      );

      expect(result, isA<ResultFailure<Order>>());
      expect(
        (result as ResultFailure<Order>).failure,
        isA<ValidationFailure>(),
      );
      expect(checkoutRepository.callCount, 0);
      expect(cartRepository.cleared, isFalse);
    },
  );

  test(
    'kargo firması seçilmemişse ValidationFailure döner ve repository çağrılmaz',
    () async {
      final checkoutRepository = _FakeCheckoutRepository();
      final cartRepository = _FakeCartRepository();
      final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

      final result = await useCase(
        addressId: 'address-1',
        shippingCompanyId: null,
        items: [item],
        cardInfo: cardInfo,
      );

      expect(result, isA<ResultFailure<Order>>());
      expect(
        (result as ResultFailure<Order>).failure,
        isA<ValidationFailure>(),
      );
      expect(checkoutRepository.callCount, 0);
    },
  );

  test('boş sepet ValidationFailure döner', () async {
    final checkoutRepository = _FakeCheckoutRepository();
    final cartRepository = _FakeCartRepository();
    final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

    final result = await useCase(
      addressId: 'address-1',
      shippingCompanyId: 'shipping-1',
      items: const [],
      cardInfo: cardInfo,
    );

    expect(result, isA<ResultFailure<Order>>());
    expect(checkoutRepository.callCount, 0);
  });

  test('geçerli veriyle sipariş tamamlanır ve sepet temizlenir', () async {
    final checkoutRepository = _FakeCheckoutRepository();
    final cartRepository = _FakeCartRepository();
    final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

    final result = await useCase(
      addressId: 'address-1',
      shippingCompanyId: 'shipping-1',
      items: [item],
      cardInfo: cardInfo,
    );

    expect(result, isA<Success<Order>>());
    expect((result as Success<Order>).value.total, 200);
    expect(checkoutRepository.callCount, 1);
    expect(cartRepository.cleared, isTrue);
  });
}
