import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/cart_item.dart';
import '../../domain/repositories/cart_repository.dart';
import '../datasources/cart_remote_data_source.dart';
import '../datasources/local_cart_data_source.dart';
import '../datasources/price_remote_data_source.dart';
import '../models/cart_item_snapshot.dart';

class CartRepositoryImpl implements CartRepository {
  CartRepositoryImpl(
    this._remoteDataSource,
    this._priceDataSource,
    this._localDataSource,
  );
  final CartRemoteDataSource _remoteDataSource;
  final PriceRemoteDataSource _priceDataSource;
  final LocalCartDataSource _localDataSource;

  @override
  Future<Result<List<CartItem>>> getCartItems() async {
    try {
      return Result.success(await _buildCartItems());
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Sepet alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<List<CartItem>>> addToCart({
    required String sellableItemId,
    required String title,
    required String imageUrl,
    int quantity = 1,
  }) async {
    try {
      final unitPrice = await _priceDataSource.getPrice(sellableItemId);
      final itemId = await _remoteDataSource.addItem(
        sellableItemId: sellableItemId,
        quantity: quantity,
      );
      await _localDataSource.saveSnapshot(
        itemId,
        CartItemSnapshot(
          title: title,
          imageUrl: imageUrl,
          unitPrice: unitPrice,
        ),
      );
      return Result.success(await _buildCartItems());
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Ürün sepete eklenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<List<CartItem>>> removeFromCart(String itemId) async {
    try {
      await _remoteDataSource.removeItem(itemId);
      await _localDataSource.removeSnapshot(itemId);
      return Result.success(await _buildCartItems());
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Ürün sepetten kaldırılırken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<List<CartItem>>> updateQuantity({
    required String itemId,
    required int quantity,
  }) async {
    try {
      if (quantity == 0) {
        await _remoteDataSource.removeItem(itemId);
        await _localDataSource.removeSnapshot(itemId);
      } else {
        await _remoteDataSource.updateItemQuantity(
          itemId: itemId,
          quantity: quantity,
        );
      }
      return Result.success(await _buildCartItems());
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Sepet güncellenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<List<CartItem>>> clearCart() async {
    try {
      await _remoteDataSource.clearCart();
      await _localDataSource.clearSnapshots();
      return const Result.success([]);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Sepet temizlenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  Future<List<CartItem>> _buildCartItems() async {
    final remoteItems = await _remoteDataSource.getCartItems();
    final snapshots = await _localDataSource.getSnapshots();
    return remoteItems
        .map(
          (item) => CartItem(
            id: item.id,
            sellableItemId: item.sellableItemId,
            sellableItemType: item.sellableItemType,
            quantity: item.quantity,
            title: snapshots[item.id]?.title ?? 'Ürün',
            imageUrl: snapshots[item.id]?.imageUrl ?? '',
            unitPrice: snapshots[item.id]?.unitPrice ?? 0,
          ),
        )
        .toList(growable: false);
  }
}
