import 'package:commerce_mobile/core/utils/result.dart';
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

    semantics.dispose();
  });
}
