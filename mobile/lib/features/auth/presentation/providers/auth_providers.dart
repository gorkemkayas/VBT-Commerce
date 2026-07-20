import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/dio_client.dart';
import '../../../../core/services/anonymous_id_service.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/services/storage_service.dart';
import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/auth_local_data_source.dart';
import '../../data/datasources/auth_remote_data_source.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/usecases/login_use_case.dart';
import '../../domain/usecases/logout_use_case.dart';
import '../../domain/usecases/register_use_case.dart';

final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>(
  (ref) => AuthRemoteDataSourceImpl(
    ref.watch(dioProvider),
    ref.watch(anonymousIdServiceProvider),
  ),
);
final authLocalDataSourceProvider = Provider<AuthLocalDataSource>(
  (ref) => AuthLocalDataSourceImpl(
    ref.watch(storageServiceProvider),
    ref.watch(secureStorageServiceProvider),
  ),
);
final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => AuthRepositoryImpl(
    ref.watch(authRemoteDataSourceProvider),
    ref.watch(authLocalDataSourceProvider),
  ),
);
final loginUseCaseProvider = Provider<LoginUseCase>(
  (ref) => LoginUseCase(ref.watch(authRepositoryProvider)),
);
final logoutUseCaseProvider = Provider<LogoutUseCase>(
  (ref) => LogoutUseCase(ref.watch(authRepositoryProvider)),
);
final registerUseCaseProvider = Provider<RegisterUseCase>(
  (ref) => RegisterUseCase(ref.watch(authRepositoryProvider)),
);

class LoginState {
  const LoginState({this.isLoading = false, this.failure, this.user});
  final bool isLoading;
  final Failure? failure;
  final User? user;
}

class LoginController extends Notifier<LoginState> {
  @override
  LoginState build() => const LoginState();

  Future<void> login({required String email, required String password}) async {
    state = const LoginState(isLoading: true);
    final result = await ref.read(loginUseCaseProvider)(
      email: email,
      password: password,
    );
    state = switch (result) {
      Success<User>(:final value) => LoginState(user: value),
      ResultFailure<User>(:final failure) => LoginState(failure: failure),
    };
  }
}

final loginControllerProvider = NotifierProvider<LoginController, LoginState>(
  LoginController.new,
);

class RegisterState {
  const RegisterState({this.isLoading = false, this.failure, this.user});
  final bool isLoading;
  final Failure? failure;
  final User? user;
}

class RegisterController extends Notifier<RegisterState> {
  @override
  RegisterState build() => const RegisterState();

  Future<void> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  }) async {
    state = const RegisterState(isLoading: true);
    final result = await ref.read(registerUseCaseProvider)(
      email: email,
      password: password,
      firstName: firstName,
      lastName: lastName,
    );
    state = switch (result) {
      Success<User>(:final value) => RegisterState(user: value),
      ResultFailure<User>(:final failure) => RegisterState(failure: failure),
    };
  }
}

final registerControllerProvider =
    NotifierProvider<RegisterController, RegisterState>(RegisterController.new);
