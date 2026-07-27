import 'package:commerce_mobile/core/utils/result.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/coupon.dart';
import 'package:commerce_mobile/features/checkout/domain/entities/shipping_company.dart';
import 'package:commerce_mobile/features/checkout/presentation/providers/checkout_providers.dart';
import 'package:commerce_mobile/features/checkout/presentation/widgets/coupon_input.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // `CouponInput`, checkout ekranının beyaz ekrana düşmesine neden olan
  // semantics doğrulama hatasını (`!semantics.parentDataDirty`) tekrar
  // tetiklemesin diye: semantics açıkken (asıl assert bu aşamada çalışır)
  // widget sorunsuz kurulmalı.
  testWidgets('semantics açıkken hata fırlatmadan kurulur', (tester) async {
    final semantics = tester.ensureSemantics();
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          // CheckoutController.build, shippingCompaniesProvider'ı dinler;
          // dio'ya gitmemek için sahte boş liste ile override edilir.
          shippingCompaniesProvider.overrideWith(
            (ref) async => const Result<List<ShippingCompany>>.success([]),
          ),
          // Kupon önerileri de dio'ya çıkar (`GET /api/coupons/active`).
          activeCouponsProvider.overrideWith((ref) async => const <Coupon>[]),
        ],
        child: const MaterialApp(
          home: Scaffold(
            body: SingleChildScrollView(child: CouponInput()),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Uygula'), findsOneWidget);
    // Aktif kupon yokken öneri bölümü hiç görünmemeli.
    expect(find.text('Uygulanabilir kuponlar'), findsNothing);

    semantics.dispose();
  });

  testWidgets('aktif kuponlar öneri çipi olarak listelenir', (tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          shippingCompaniesProvider.overrideWith(
            (ref) async => const Result<List<ShippingCompany>>.success([]),
          ),
          activeCouponsProvider.overrideWith(
            (ref) async => const [
              Coupon(
                id: 'coupon-1',
                code: 'BAHAR10',
                discountType: CouponDiscountType.percentage,
                discountValue: 10,
              ),
              Coupon(
                id: 'coupon-2',
                code: 'KARGO50',
                discountType: CouponDiscountType.fixedAmount,
                discountValue: 50,
              ),
            ],
          ),
        ],
        child: const MaterialApp(
          home: Scaffold(
            body: SingleChildScrollView(child: CouponInput()),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Uygulanabilir kuponlar'), findsOneWidget);
    expect(find.text('BAHAR10 · %10'), findsOneWidget);
    expect(find.text('KARGO50 · 50,00 ₺'), findsOneWidget);
  });
}
