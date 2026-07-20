import 'package:dio/dio.dart';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/anonymous_id_service.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../models/remote_cart_item.dart';

/// Backend'de sepetin gerçek kaynağı `"Variant"` tipindeki satılabilir
/// varyantlar üzerinden çalışır — Product feature bugün yalnızca bunu
/// destekliyor.
const cartSellableItemType = 'Variant';

abstract interface class CartRemoteDataSource {
  Future<List<RemoteCartItem>> getCartItems();
  Future<String> addItem({
    required String sellableItemId,
    required int quantity,
  });
  Future<void> updateItemQuantity({
    required String itemId,
    required int quantity,
  });
  Future<void> removeItem(String itemId);
  Future<void> clearCart();
}

/// Kullanıcı giriş yapmışsa `/api/carts/me`, yapmamışsa `/api/carts/anonymous/{id}`
/// uçlarını kullanır. Giriş durumu, access token'ın secure storage'da olup
/// olmamasına bakılarak belirlenir (Authorization header'ı zaten
/// `AuthInterceptor` tarafından otomatik ekleniyor).
class CartRemoteDataSourceImpl implements CartRemoteDataSource {
  CartRemoteDataSourceImpl(
    this._dio,
    this._secureStorage,
    this._anonymousIdService,
  );
  final Dio _dio;
  final SecureStorageService _secureStorage;
  final AnonymousIdService _anonymousIdService;

  Future<String> _cartBasePath() async {
    final accessToken = await _secureStorage.getString(StorageKeys.accessToken);
    if (accessToken != null) return '/api/carts/me';
    final anonymousId = await _anonymousIdService.getOrCreateAnonymousId();
    return '/api/carts/anonymous/$anonymousId';
  }

  @override
  Future<List<RemoteCartItem>> getCartItems() async {
    final basePath = await _cartBasePath();
    final response = await _dio.get<Map<String, dynamic>>(basePath);
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş sepet verisi alındı.');
    }
    final items = body['items'];
    if (items is! List) {
      throw const FormatException('Sepet yanıtı beklenen şekilde değil.');
    }
    return items
        .map((item) => RemoteCartItem.fromJson(item as Map<String, dynamic>))
        .toList(growable: false);
  }

  @override
  Future<String> addItem({
    required String sellableItemId,
    required int quantity,
  }) async {
    final basePath = await _cartBasePath();
    final response = await _dio.post<dynamic>(
      '$basePath/items',
      data: {
        'sellableItemId': sellableItemId,
        'sellableItemType': cartSellableItemType,
        'quantity': quantity,
      },
    );
    final id = response.data;
    if (id is! String || id.isEmpty) {
      throw const FormatException(
        'Ürün sepete eklenirken sunucudan geçersiz yanıt alındı.',
      );
    }
    return id;
  }

  @override
  Future<void> updateItemQuantity({
    required String itemId,
    required int quantity,
  }) async {
    final basePath = await _cartBasePath();
    await _dio.put<void>(
      '$basePath/items/$itemId',
      data: {'quantity': quantity},
    );
  }

  @override
  Future<void> removeItem(String itemId) async {
    final basePath = await _cartBasePath();
    await _dio.delete<void>('$basePath/items/$itemId');
  }

  @override
  Future<void> clearCart() async {
    final basePath = await _cartBasePath();
    await _dio.delete<void>(basePath);
  }
}
