import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/customer.dart';
import '../../domain/entities/customer_address.dart';
import '../../domain/repositories/customer_repository.dart';
import '../datasources/customer_remote_data_source.dart';

class CustomerRepositoryImpl implements CustomerRepository {
  CustomerRepositoryImpl(this._remoteDataSource);
  final CustomerRemoteDataSource _remoteDataSource;

  @override
  Future<Result<Customer>> getCurrentCustomer() async {
    try {
      return Result.success(await _remoteDataSource.getCurrentCustomer());
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Müşteri bilgileri alınırken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<String>> addAddress(CustomerAddressInput input) async {
    try {
      return Result.success(await _remoteDataSource.addAddress(input));
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Adres eklenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> updateAddress(
    String addressId,
    CustomerAddressInput input,
  ) async {
    try {
      await _remoteDataSource.updateAddress(addressId, input);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Adres güncellenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> deleteAddress(String addressId) async {
    try {
      await _remoteDataSource.deleteAddress(addressId);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Adres silinirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> setDefaultAddress(String addressId) async {
    try {
      await _remoteDataSource.setDefaultAddress(addressId);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Varsayılan adres ayarlanırken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }
}
