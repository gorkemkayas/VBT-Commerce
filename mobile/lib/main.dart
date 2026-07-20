import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'app.dart';
import 'core/services/anonymous_id_service.dart';
import 'core/services/storage_service.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
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
