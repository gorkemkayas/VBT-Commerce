import '../../../../core/utils/result.dart';
import '../entities/category.dart';
import '../repositories/product_repository.dart';

class GetCategoriesUseCase {
  const GetCategoriesUseCase(this._repository);
  final ProductRepository _repository;
  Future<Result<List<Category>>> call() => _repository.getCategories();
}
