import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/price_calculation.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../datasources/order_remote_data_source.dart';
import '../datasources/pricing_remote_data_source.dart';
import '../datasources/shipping_company_remote_data_source.dart';

/// `addressId`, Customer feature'ındaki kayıtlı adreslerden seçilir (bkz.
/// `CheckoutController.selectAddress`) — bu akışa dokunulmuyor.
///
/// Kargo firması seçim UI'ı ve ödeme formu bu görevin kapsamı dışında;
/// backend'in zorunlu tuttuğu bu alanlar için sırasıyla aktif firmalardan
/// ilki ve sabit placeholder kart bilgisi kullanılır (bkz.
/// `ShippingCompanyRemoteDataSource`, `OrderRemoteDataSource`). Bunlar ayrı
/// bir görevde gerçek seçim/form ile değiştirilecek.
class CheckoutRepositoryImpl implements CheckoutRepository {
  CheckoutRepositoryImpl(
    this._orderDataSource,
    this._shippingDataSource,
    this._pricingDataSource,
  );
  final OrderRemoteDataSource _orderDataSource;
  final ShippingCompanyRemoteDataSource _shippingDataSource;
  final PricingRemoteDataSource _pricingDataSource;

  @override
  Future<Result<Order>> completeOrder({
    required String addressId,
    required List<CartItem> items,
  }) async {
    try {
      final shippingCompanyId = await _shippingDataSource.getFirstActiveId();
      if (shippingCompanyId == null) {
        return const Result.failure(
          ServerFailure(
            'Şu anda kullanılabilir bir kargo firması bulunamadı. Lütfen daha sonra tekrar deneyin.',
          ),
        );
      }
      final orderId = await _orderDataSource.placeMyOrder(
        addressId: addressId,
        shippingCompanyId: shippingCompanyId,
      );
      final total = items.fold(0.0, (total, item) => total + item.lineTotal);
      return Result.success(
        Order(
          orderId: orderId,
          placedAt: DateTime.now(),
          items: items,
          total: total,
        ),
      );
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Sipariş oluşturulurken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<PriceCalculation>> calculatePrice(List<CartItem> items) async {
    try {
      final result = await _pricingDataSource.calculateMy(items);
      return Result.success(result);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Fiyat hesaplanırken beklenmeyen bir hata oluştu.'),
      );
    }
  }
}
