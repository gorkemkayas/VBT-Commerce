import 'package:dio/dio.dart';

import '../../../cart/domain/entities/cart_item.dart';
import '../models/price_calculation_result_model.dart';

abstract interface class PricingRemoteDataSource {
  Future<PriceCalculationResultModel> calculateMy(List<CartItem> items);
  Future<PriceCalculationResultModel> calculateGuest(
    String guestCustomerId,
    List<CartItem> items,
  );
}

/// `POST /api/pricing/calculate/me`. Kupon kodu girişi bu görevin kapsamı
/// dışında; `couponCodes` her zaman boş liste gönderilir.
class PricingRemoteDataSourceImpl implements PricingRemoteDataSource {
  PricingRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<PriceCalculationResultModel> calculateMy(List<CartItem> items) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/pricing/calculate/me',
      data: {
        'items': items.map(_itemJson).toList(),
        'couponCodes': <String>[],
      },
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException(
        'Sunucudan boş fiyat hesaplama yanıtı alındı.',
      );
    }
    return PriceCalculationResultModel.fromJson(body);
  }

  /// `POST /api/pricing/calculate/guest`. Yanıt şekli `calculateMy` ile
  /// aynıdır (`PriceCalculationResultDto`).
  @override
  Future<PriceCalculationResultModel> calculateGuest(
    String guestCustomerId,
    List<CartItem> items,
  ) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/pricing/calculate/guest',
      data: {
        'guestCustomerId': guestCustomerId,
        'items': items.map(_itemJson).toList(),
        'couponCodes': <String>[],
      },
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException(
        'Sunucudan boş fiyat hesaplama yanıtı alındı.',
      );
    }
    return PriceCalculationResultModel.fromJson(body);
  }

  Map<String, dynamic> _itemJson(CartItem item) => {
    'sellableItemId': item.sellableItemId,
    'sellableItemType': item.sellableItemType,
    'quantity': item.quantity,
  };
}
