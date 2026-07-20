import '../../../../core/utils/result.dart';
import '../entities/product.dart';
import '../entities/product_filter.dart';
import '../repositories/product_repository.dart';

class FilterProductsUseCase {
  const FilterProductsUseCase(this._repository);
  final ProductRepository _repository;
  Future<Result<List<Product>>> call(ProductFilter filter) =>
      _repository.filterProducts(filter);
}
