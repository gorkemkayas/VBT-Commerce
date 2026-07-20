import '../../../../core/utils/result.dart';
import '../repositories/product_repository.dart';

class GetCategoriesUseCase {
  const GetCategoriesUseCase(this._repository);
  final ProductRepository _repository;
  Future<Result<List<String>>> call() => _repository.getCategories();
}
