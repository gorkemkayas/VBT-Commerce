import '../../../../core/utils/result.dart';
import '../entities/category.dart';
import '../entities/product.dart';
import '../entities/product_filter.dart';

abstract interface class ProductRepository {
  Future<Result<List<Product>>> getProducts();
  Future<Result<Product>> getProductDetail(String id);
  Future<Result<List<Category>>> getCategories();
  Future<Result<List<Product>>> searchProducts(String query);
  Future<Result<List<Product>>> filterProducts(ProductFilter filter);
}
