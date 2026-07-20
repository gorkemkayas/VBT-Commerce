import 'package:flutter/widgets.dart';

/// `GoRouter`'a ve `core/network`'teki interceptor'lara ortak, tarafsız bir
/// referans sağlar — böylece network katmanı, router dosyasına doğrudan
/// bağımlı olmadan (döngüsel import olmadan) navigasyon tetikleyebilir.
final rootNavigatorKey = GlobalKey<NavigatorState>();
