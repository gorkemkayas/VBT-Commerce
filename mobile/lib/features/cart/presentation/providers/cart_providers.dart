import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/services/anonymous_id_service.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/services/storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/cart_remote_data_source.dart';
import '../../data/datasources/local_cart_data_source.dart';
import '../../data/datasources/price_remote_data_source.dart';
import '../../data/repositories/cart_repository_impl.dart';
import '../../domain/entities/cart_item.dart';
import '../../domain/repositories/cart_repository.dart';
import '../../domain/usecases/add_to_cart_use_case.dart';
import '../../domain/usecases/clear_cart_use_case.dart';
import '../../domain/usecases/get_cart_items_use_case.dart';
import '../../domain/usecases/remove_from_cart_use_case.dart';
import '../../domain/usecases/update_quantity_use_case.dart';

final cartRemoteDataSourceProvider = Provider<CartRemoteDataSource>(
  (ref) => CartRemoteDataSourceImpl(
    ref.watch(dioProvider),
    ref.watch(secureStorageServiceProvider),
    ref.watch(anonymousIdServiceProvider),
  ),
);
final priceRemoteDataSourceProvider = Provider<PriceRemoteDataSource>(
  (ref) => PriceRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final localCartDataSourceProvider = Provider<LocalCartDataSource>(
  (ref) => LocalCartDataSourceImpl(ref.watch(storageServiceProvider)),
);
final cartRepositoryProvider = Provider<CartRepository>(
  (ref) => CartRepositoryImpl(
    ref.watch(cartRemoteDataSourceProvider),
    ref.watch(priceRemoteDataSourceProvider),
    ref.watch(localCartDataSourceProvider),
  ),
);
final addToCartUseCaseProvider = Provider<AddToCartUseCase>(
  (ref) => AddToCartUseCase(ref.watch(cartRepositoryProvider)),
);
final removeFromCartUseCaseProvider = Provider<RemoveFromCartUseCase>(
  (ref) => RemoveFromCartUseCase(ref.watch(cartRepositoryProvider)),
);
final updateQuantityUseCaseProvider = Provider<UpdateQuantityUseCase>(
  (ref) => UpdateQuantityUseCase(ref.watch(cartRepositoryProvider)),
);
final getCartItemsUseCaseProvider = Provider<GetCartItemsUseCase>(
  (ref) => GetCartItemsUseCase(ref.watch(cartRepositoryProvider)),
);
final clearCartUseCaseProvider = Provider<ClearCartUseCase>(
  (ref) => ClearCartUseCase(ref.watch(cartRepositoryProvider)),
);

class CartState {
  const CartState({
    this.isLoading = false,
    this.items = const [],
    this.failure,
  });
  final bool isLoading;
  final List<CartItem> items;
  final Failure? failure;
  int get itemCount => items.fold(0, (total, item) => total + item.quantity);
  double get subtotal => items.fold(0, (total, item) => total + item.lineTotal);
}

class CartController extends Notifier<CartState> {
  @override
  CartState build() {
    Future.microtask(loadCart);
    return const CartState(isLoading: true);
  }

  Future<void> loadCart() => _apply(ref.read(getCartItemsUseCaseProvider)());
  Future<void> add({
    required String sellableItemId,
    required String title,
    required String imageUrl,
    int quantity = 1,
  }) => _apply(
    ref.read(addToCartUseCaseProvider)(
      sellableItemId: sellableItemId,
      title: title,
      imageUrl: imageUrl,
      quantity: quantity,
    ),
  );
  Future<void> remove(String itemId) =>
      _apply(ref.read(removeFromCartUseCaseProvider)(itemId));
  Future<void> updateQuantity({
    required String itemId,
    required int quantity,
  }) => _apply(
    ref.read(updateQuantityUseCaseProvider)(itemId: itemId, quantity: quantity),
  );
  Future<void> clear() => _apply(ref.read(clearCartUseCaseProvider)());

  Future<void> _apply(Future<Result<List<CartItem>>> operation) async {
    state = CartState(isLoading: true, items: state.items);
    final result = await operation;
    state = switch (result) {
      Success<List<CartItem>>(:final value) => CartState(items: value),
      ResultFailure<List<CartItem>>(:final failure) => CartState(
        items: state.items,
        failure: failure,
      ),
    };
  }
}

final cartControllerProvider = NotifierProvider<CartController, CartState>(
  CartController.new,
);
