import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/services/storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/local_favorites_data_source.dart';
import '../../data/repositories/favorites_repository_impl.dart';
import '../../domain/entities/favorite_item.dart';
import '../../domain/repositories/favorites_repository.dart';
import '../../domain/usecases/add_favorite_use_case.dart';
import '../../domain/usecases/get_favorites_use_case.dart';
import '../../domain/usecases/remove_favorite_use_case.dart';

final localFavoritesDataSourceProvider = Provider<LocalFavoritesDataSource>(
  (ref) => LocalFavoritesDataSourceImpl(ref.watch(storageServiceProvider)),
);
final favoritesRepositoryProvider = Provider<FavoritesRepository>(
  (ref) => FavoritesRepositoryImpl(ref.watch(localFavoritesDataSourceProvider)),
);
final getFavoritesUseCaseProvider = Provider<GetFavoritesUseCase>(
  (ref) => GetFavoritesUseCase(ref.watch(favoritesRepositoryProvider)),
);
final addFavoriteUseCaseProvider = Provider<AddFavoriteUseCase>(
  (ref) => AddFavoriteUseCase(ref.watch(favoritesRepositoryProvider)),
);
final removeFavoriteUseCaseProvider = Provider<RemoveFavoriteUseCase>(
  (ref) => RemoveFavoriteUseCase(ref.watch(favoritesRepositoryProvider)),
);

class FavoritesState {
  const FavoritesState({
    this.isLoading = false,
    this.items = const [],
    this.failure,
  });

  final bool isLoading;
  final List<FavoriteItem> items;
  final Failure? failure;

  bool isFavorite(String productId) =>
      items.any((item) => item.productId == productId);
}

class FavoritesController extends Notifier<FavoritesState> {
  @override
  FavoritesState build() {
    Future.microtask(load);
    return const FavoritesState(isLoading: true);
  }

  Future<void> load() => _apply(ref.read(getFavoritesUseCaseProvider)());

  /// Ürün favorideyse çıkarır, değilse ekler. Buton anlık geri bildirim için
  /// bu metodu bekleyebilir.
  Future<void> toggle(FavoriteItem item) {
    if (state.isFavorite(item.productId)) {
      return _apply(ref.read(removeFavoriteUseCaseProvider)(item.productId));
    }
    return _apply(ref.read(addFavoriteUseCaseProvider)(item));
  }

  Future<void> remove(String productId) =>
      _apply(ref.read(removeFavoriteUseCaseProvider)(productId));

  Future<void> _apply(Future<Result<List<FavoriteItem>>> operation) async {
    state = FavoritesState(isLoading: true, items: state.items);
    final result = await operation;
    state = switch (result) {
      Success<List<FavoriteItem>>(:final value) => FavoritesState(items: value),
      ResultFailure<List<FavoriteItem>>(:final failure) => FavoritesState(
        items: state.items,
        failure: failure,
      ),
    };
  }
}

final favoritesControllerProvider =
    NotifierProvider<FavoritesController, FavoritesState>(
      FavoritesController.new,
    );
