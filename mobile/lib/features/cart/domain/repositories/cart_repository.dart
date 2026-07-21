import '../../../../core/utils/result.dart';
import '../entities/cart_item.dart';

abstract interface class CartRepository {
  Future<Result<List<CartItem>>> getCartItems();

  Future<Result<List<CartItem>>> addToCart({
    required String sellableItemId,
    required bool isVariant,
    required String title,
    required String imageUrl,
    int quantity = 1,
  });

  Future<Result<List<CartItem>>> removeFromCart(String itemId);

  Future<Result<List<CartItem>>> updateQuantity({
    required String itemId,
    required int quantity,
  });

  Future<Result<List<CartItem>>> clearCart();
}
