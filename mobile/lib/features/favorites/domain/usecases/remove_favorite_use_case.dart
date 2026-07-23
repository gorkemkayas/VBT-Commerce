import '../../../../core/utils/result.dart';
import '../entities/favorite_item.dart';
import '../repositories/favorites_repository.dart';

class RemoveFavoriteUseCase {
  const RemoveFavoriteUseCase(this._repository);
  final FavoritesRepository _repository;
  Future<Result<List<FavoriteItem>>> call(String productId) =>
      _repository.remove(productId);
}
