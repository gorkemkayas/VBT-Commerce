import '../../../../core/utils/result.dart';
import '../entities/favorite_item.dart';

/// Favoriler yalnızca yerelde saklanır (backend uç noktası yok). Yazma
/// işlemleri güncel favori listesini döndürür, böylece sunum katmanı tek
/// adımda durumunu tazeleyebilir.
abstract interface class FavoritesRepository {
  Future<Result<List<FavoriteItem>>> getFavorites();
  Future<Result<List<FavoriteItem>>> add(FavoriteItem item);
  Future<Result<List<FavoriteItem>>> remove(String productId);
}
