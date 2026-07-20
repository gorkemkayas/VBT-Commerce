import 'package:commerce_mobile/core/errors/failure.dart';
import 'package:commerce_mobile/core/utils/result.dart';
import 'package:commerce_mobile/features/cart/domain/entities/cart_item.dart';
import 'package:commerce_mobile/features/cart/domain/repositories/cart_repository.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/address.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/order.dart';
import 'package:commerce_mobile/features/checkout/domain/repositories/checkout_repository.dart';
import 'package:commerce_mobile/features/checkout/domain/usecases/complete_order_use_case.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeCheckoutRepository implements CheckoutRepository {
  int callCount = 0;

  @override
  Future<Result<Order>> completeOrder({
    required Address address,
    required List<CartItem> items,
  }) async {
    callCount++;
    return Result.success(
      Order(
        orderId: 'TEST-1',
        placedAt: DateTime(2026),
        address: address,
        items: items,
        total: items.fold(0, (total, item) => total + item.lineTotal),
      ),
    );
  }
}

class _FakeCartRepository implements CartRepository {
  bool cleared = false;

  @override
  Future<Result<List<CartItem>>> addToCart({
    required String sellableItemId,
    required String title,
    required String imageUrl,
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
  const validAddress = Address(
    fullName: 'Ada Lovelace',
    phone: '5551234567',
    addressLine: 'Test Sokak No:1',
    city: 'İstanbul',
    postalCode: '34000',
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
    'geçersiz adres ValidationFailure döner ve repository çağrılmaz',
    () async {
      final checkoutRepository = _FakeCheckoutRepository();
      final cartRepository = _FakeCartRepository();
      final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

      final result = await useCase(address: Address.empty, items: [item]);

      expect(result, isA<ResultFailure<Order>>());
      expect(
        (result as ResultFailure<Order>).failure,
        isA<ValidationFailure>(),
      );
      expect(checkoutRepository.callCount, 0);
      expect(cartRepository.cleared, isFalse);
    },
  );

  test('boş sepet ValidationFailure döner', () async {
    final checkoutRepository = _FakeCheckoutRepository();
    final cartRepository = _FakeCartRepository();
    final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

    final result = await useCase(address: validAddress, items: const []);

    expect(result, isA<ResultFailure<Order>>());
    expect(checkoutRepository.callCount, 0);
  });

  test('geçerli veriyle sipariş tamamlanır ve sepet temizlenir', () async {
    final checkoutRepository = _FakeCheckoutRepository();
    final cartRepository = _FakeCartRepository();
    final useCase = CompleteOrderUseCase(checkoutRepository, cartRepository);

    final result = await useCase(address: validAddress, items: [item]);

    expect(result, isA<Success<Order>>());
    expect((result as Success<Order>).value.total, 200);
    expect(checkoutRepository.callCount, 1);
    expect(cartRepository.cleared, isTrue);
  });
}
