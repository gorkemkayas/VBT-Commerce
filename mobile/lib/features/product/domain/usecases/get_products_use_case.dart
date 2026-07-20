import '../../../../core/utils/result.dart';
import '../entities/product.dart';
import '../repositories/product_repository.dart';

class GetProductsUseCase {
  const GetProductsUseCase(this._repository);
  final ProductRepository _repository;
  Future<Result<List<Product>>> call() => _repository.getProducts();
}
