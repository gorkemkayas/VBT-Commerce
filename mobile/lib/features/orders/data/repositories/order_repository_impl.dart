import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/order.dart';
import '../../domain/repositories/order_repository.dart';
import '../datasources/order_remote_data_source.dart';

class OrderRepositoryImpl implements OrderRepository {
  OrderRepositoryImpl(this._remoteDataSource);
  final OrderRemoteDataSource _remoteDataSource;

  @override
  Future<Result<List<Order>>> getMyOrders() async {
    try {
      final orders = await _remoteDataSource.getMyOrders();
      // Backend sıralama garantisi vermiyor; en yeni sipariş en üstte olsun.
      final sorted = [...orders]
        ..sort((first, second) => second.createdAt.compareTo(first.createdAt));
      return Result.success(sorted);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Siparişler alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<Order>> getOrderById(String orderId) async {
    try {
      return Result.success(await _remoteDataSource.getOrderById(orderId));
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Sipariş detayı alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> cancelOrder(String orderId) async {
    try {
      await _remoteDataSource.cancelOrder(orderId);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Sipariş iptal edilirken beklenmeyen bir hata oluştu.'),
      );
    }
  }
}
