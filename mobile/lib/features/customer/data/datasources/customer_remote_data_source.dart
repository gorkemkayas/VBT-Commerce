import 'package:dio/dio.dart';

import '../../domain/entities/customer_address.dart';
import '../models/customer_model.dart';

abstract interface class CustomerRemoteDataSource {
  Future<CustomerModel> getCurrentCustomer();
  Future<void> updateProfile({String? phoneNumber, String? dateOfBirth});
  Future<String> addAddress(CustomerAddressInput input);
  Future<void> updateAddress(String addressId, CustomerAddressInput input);
  Future<void> deleteAddress(String addressId);
  Future<void> setDefaultAddress(String addressId);
}

class CustomerRemoteDataSourceImpl implements CustomerRemoteDataSource {
  CustomerRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  @override
  Future<CustomerModel> getCurrentCustomer() async {
    final response = await _dio.get<Map<String, dynamic>>('/api/customers/me');
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş müşteri verisi alındı.');
    }
    return CustomerModel.fromJson(body);
  }

  @override
  Future<void> updateProfile({
    String? phoneNumber,
    String? dateOfBirth,
  }) async {
    await _dio.put<void>(
      '/api/customers/me',
      data: {'phoneNumber': phoneNumber, 'dateOfBirth': dateOfBirth},
    );
  }

  @override
  Future<String> addAddress(CustomerAddressInput input) async {
    final response = await _dio.post<dynamic>(
      '/api/customers/me/addresses',
      data: _addressJson(input),
    );
    final id = response.data;
    if (id is! String || id.isEmpty) {
      throw const FormatException(
        'Adres eklenirken sunucudan geçersiz yanıt alındı.',
      );
    }
    return id;
  }

  @override
  Future<void> updateAddress(
    String addressId,
    CustomerAddressInput input,
  ) async {
    await _dio.put<void>(
      '/api/customers/me/addresses/$addressId',
      data: _addressJson(input),
    );
  }

  @override
  Future<void> deleteAddress(String addressId) async {
    await _dio.delete<void>('/api/customers/me/addresses/$addressId');
  }

  @override
  Future<void> setDefaultAddress(String addressId) async {
    await _dio.post<void>('/api/customers/me/addresses/$addressId/set-default');
  }

  Map<String, dynamic> _addressJson(CustomerAddressInput input) => {
    'label': input.label,
    'recipientName': input.recipientName,
    'phoneNumber': input.phoneNumber,
    'country': input.country,
    'city': input.city,
    'district': input.district,
    'postalCode': input.postalCode,
    'addressLine1': input.addressLine1,
    'addressLine2': input.addressLine2,
    'isDefault': input.isDefault,
  };
}
