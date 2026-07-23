import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'app.dart';
import 'core/services/anonymous_id_service.dart';
import 'core/services/storage_service.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  // `DateFormat`'ın Türkçe ay/gün adları için locale verisi; yüklenmezse
  // `tr_TR` ile kurulan formatlayıcılar çalışma anında hata verir.
  await initializeDateFormatting('tr_TR', null);
  final preferences = await SharedPreferences.getInstance();

  final container = ProviderContainer(
    overrides: [sharedPreferencesProvider.overrideWithValue(preferences)],
  );
  // Misafir sepeti kimliği ilk açılışta oluşturulup saklanır.
  await container.read(anonymousIdServiceProvider).getOrCreateAnonymousId();

  runApp(
    UncontrolledProviderScope(
      container: container,
      child: const SneakerStoreApp(),
    ),
  );
}
