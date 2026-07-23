import '../../../../core/utils/result.dart';
import '../entities/favorite_item.dart';
import '../repositories/favorites_repository.dart';

class AddFavoriteUseCase {
  const AddFavoriteUseCase(this._repository);
  final FavoritesRepository _repository;
  Future<Result<List<FavoriteItem>>> call(FavoriteItem item) =>
      _repository.add(item);
}
