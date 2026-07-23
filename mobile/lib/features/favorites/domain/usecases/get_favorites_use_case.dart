import '../../../../core/utils/result.dart';
import '../entities/favorite_item.dart';
import '../repositories/favorites_repository.dart';

class GetFavoritesUseCase {
  const GetFavoritesUseCase(this._repository);
  final FavoritesRepository _repository;
  Future<Result<List<FavoriteItem>>> call() => _repository.getFavorites();
}
