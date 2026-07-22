import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/order_remote_data_source.dart';
import '../../data/repositories/order_repository_impl.dart';
import '../../domain/entities/order.dart';
import '../../domain/repositories/order_repository.dart';
import '../../domain/usecases/get_my_orders_use_case.dart';
import '../../domain/usecases/get_order_by_id_use_case.dart';

final orderRemoteDataSourceProvider = Provider<OrderRemoteDataSource>(
  (ref) => OrderRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final orderRepositoryProvider = Provider<OrderRepository>(
  (ref) => OrderRepositoryImpl(ref.watch(orderRemoteDataSourceProvider)),
);
final getMyOrdersUseCaseProvider = Provider<GetMyOrdersUseCase>(
  (ref) => GetMyOrdersUseCase(ref.watch(orderRepositoryProvider)),
);
final getOrderByIdUseCaseProvider = Provider<GetOrderByIdUseCase>(
  (ref) => GetOrderByIdUseCase(ref.watch(orderRepositoryProvider)),
);

/// Sipariş detay ekranı bunu izler; `productDetailProvider` ile aynı desen
/// (autoDispose family).
final orderDetailProvider = FutureProvider.autoDispose
    .family<Result<Order>, String>((ref, orderId) {
      return ref.watch(getOrderByIdUseCaseProvider)(orderId);
    });

class OrdersState {
  const OrdersState({
    this.isLoading = false,
    this.orders = const [],
    this.failure,
  });
  final bool isLoading;
  final List<Order> orders;
  final Failure? failure;
}

class OrdersController extends Notifier<OrdersState> {
  @override
  OrdersState build() {
    Future.microtask(loadOrders);
    return const OrdersState(isLoading: true);
  }

  Future<void> loadOrders() async {
    state = OrdersState(isLoading: true, orders: state.orders);
    final result = await ref.read(getMyOrdersUseCaseProvider)();
    state = switch (result) {
      Success<List<Order>>(:final value) => OrdersState(orders: value),
      ResultFailure<List<Order>>(:final failure) => OrdersState(
        orders: state.orders,
        failure: failure,
      ),
    };
  }
}

final ordersControllerProvider =
    NotifierProvider<OrdersController, OrdersState>(OrdersController.new);
