import '../../../../core/utils/result.dart';
import '../entities/product.dart';
import '../repositories/product_repository.dart';

class SearchProductsUseCase {
  const SearchProductsUseCase(this._repository);
  final ProductRepository _repository;
  Future<Result<List<Product>>> call(String query) =>
      _repository.searchProducts(query);
}
