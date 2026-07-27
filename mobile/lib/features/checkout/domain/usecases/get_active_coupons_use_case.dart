import '../../../../core/utils/result.dart';
import '../entities/coupon.dart';
import '../repositories/checkout_repository.dart';

/// Checkout'ta önerilecek aktif kuponları getirir.
class GetActiveCouponsUseCase {
  const GetActiveCouponsUseCase(this._repository);
  final CheckoutRepository _repository;

  Future<Result<List<Coupon>>> call() => _repository.getActiveCoupons();
}
