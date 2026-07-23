import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/favorite_item.dart';
import '../../domain/repositories/favorites_repository.dart';
import '../datasources/local_favorites_data_source.dart';
import '../models/favorite_item_model.dart';

class FavoritesRepositoryImpl implements FavoritesRepository {
  FavoritesRepositoryImpl(this._localDataSource);
  final LocalFavoritesDataSource _localDataSource;

  @override
  Future<Result<List<FavoriteItem>>> getFavorites() =>
      _guard(() => _localDataSource.getFavorites());

  @override
  Future<Result<List<FavoriteItem>>> add(FavoriteItem item) => _guard(() async {
    final favorites = await _localDataSource.getFavorites();
    // Tekrar eklemeyi engelle; zaten favorideyse listeyi olduğu gibi bırak.
    if (favorites.any((existing) => existing.productId == item.productId)) {
      return favorites;
    }
    final updated = [...favorites, FavoriteItemModel.fromEntity(item)];
    await _localDataSource.saveFavorites(updated);
    return updated;
  });

  @override
  Future<Result<List<FavoriteItem>>> remove(String productId) =>
      _guard(() async {
        final favorites = await _localDataSource.getFavorites();
        final updated = favorites
            .where((item) => item.productId != productId)
            .toList(growable: false);
        await _localDataSource.saveFavorites(updated);
        return updated;
      });

  Future<Result<List<FavoriteItem>>> _guard(
    Future<List<FavoriteItemModel>> Function() operation,
  ) async {
    try {
      return Result.success(await operation());
    } on FormatException catch (error) {
      return Result.failure(CacheFailure(error.message));
    } catch (_) {
      return const Result.failure(
        CacheFailure('Favoriler işlenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }
}
