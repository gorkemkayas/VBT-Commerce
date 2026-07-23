import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../domain/entities/guest_checkout_info.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/price_calculation.dart';
import '../../domain/entities/shipping_company.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../datasources/guest_customer_remote_data_source.dart';
import '../datasources/order_remote_data_source.dart';
import '../datasources/pricing_remote_data_source.dart';
import '../datasources/shipping_company_remote_data_source.dart';

/// `addressId`, Customer feature'ındaki kayıtlı adreslerden seçilir (bkz.
/// `CheckoutController.selectAddress`) — bu akışa dokunulmuyor.
/// `shippingCompanyId`, kullanıcının Checkout ekranında seçtiği kargo
/// firmasıdır (bkz. `CheckoutController.selectShippingCompany`).
class CheckoutRepositoryImpl implements CheckoutRepository {
  CheckoutRepositoryImpl(
    this._orderDataSource,
    this._shippingDataSource,
    this._pricingDataSource,
    this._guestCustomerDataSource,
  );
  final OrderRemoteDataSource _orderDataSource;
  final ShippingCompanyRemoteDataSource _shippingDataSource;
  final PricingRemoteDataSource _pricingDataSource;
  final GuestCustomerRemoteDataSource _guestCustomerDataSource;

  @override
  Future<Result<Order>> completeOrder({
    required String addressId,
    required String shippingCompanyId,
    required List<CartItem> items,
  }) async {
    try {
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

  @override
  Future<Result<List<ShippingCompany>>> getShippingCompanies() async {
    try {
      final companies = await _shippingDataSource.getActive();
      return Result.success(companies);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Kargo firmaları alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<String>> createGuestCustomer({
    required String firstName,
    required String lastName,
    required String email,
    required String phoneNumber,
  }) async {
    try {
      return Result.success(
        await _guestCustomerDataSource.create(
          firstName: firstName,
          lastName: lastName,
          email: email,
          phoneNumber: phoneNumber,
        ),
      );
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Misafir müşteri oluşturulurken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<PriceCalculation>> calculatePriceGuest(
    String guestCustomerId,
    List<CartItem> items,
  ) async {
    try {
      final result = await _pricingDataSource.calculateGuest(
        guestCustomerId,
        items,
      );
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

  @override
  Future<Result<Order>> completeGuestOrder({
    required String guestCustomerId,
    required String anonymousId,
    required String shippingCompanyId,
    required GuestCheckoutInfo info,
    required List<CartItem> items,
  }) async {
    try {
      final orderId = await _orderDataSource.placeGuestOrder(
        guestCustomerId: guestCustomerId,
        anonymousId: anonymousId,
        shippingCompanyId: shippingCompanyId,
        recipientName: info.recipientName,
        phoneNumber: info.phoneNumber,
        country: info.country,
        city: info.city,
        district: info.district,
        postalCode: info.postalCode,
        addressLine1: info.addressLine1,
        addressLine2: info.addressLine2,
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
}
