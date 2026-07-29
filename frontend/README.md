# VBT-Commerce — Frontend

Trendora e-ticaret platformunun web arayüzü. **Next.js 16 (App Router)** ve **React 19** ile yazılmış; hem müşteri vitrinini hem de `/admin` altındaki yönetim panelini tek bir uygulama içinde barındırır. Backend'deki [ECommerce.API](../backend/README.md)'ye REST üzerinden bağlanır.

---

## 1. Teknoloji Yığını

| Alan | Teknoloji |
|---|---|
| Framework | Next.js 16 (App Router), React 19 |
| Dil | TypeScript (strict mode) |
| Stil | Tailwind CSS v4 (CSS-first config, `app/globals.css` içinde `@theme`) |
| Bileşen kütüphanesi | shadcn/ui (`components.json`, stil: `base-nova`) + Base UI primitives |
| İkonlar | lucide-react |
| State yönetimi | React Context + `useSyncExternalStore` (Redux/Zustand yok) |
| HTTP | Native `fetch`, tip güvenli sarmalayıcı: `lib/api/client.ts` |
| Fontlar | `next/font/google` — Geist, Geist Mono, Jaini, Playfair Display (₺ işareti için `latin-ext` alt kümesi) |
| Analytics | `@vercel/analytics` (yalnızca production'da mount edilir) |
| Deploy | Docker (3 aşamalı build, `output: "standalone"`) |

Proje başlangıçta [v0.app](https://v0.app) ile scaffold edilmiş (`app/layout.tsx` içindeki `generator: 'v0.app'` metadata alanı ve `.gitignore`'daki v0 sandbox girdileri bunun izleri).

## 2. Proje Yapısı

```
frontend/
├── app/                      # Next.js App Router sayfaları + route handler'lar
│   ├── page.tsx              # Ana sayfa (hero video, kategori vitrini, kupon marquee)
│   ├── shop/                 # Ürün listeleme (filtre, arama, sıralama, animasyonlu grid)
│   ├── product/[slug]/       # Ürün detay
│   ├── cart/                 # Sepet sayfası (+ global sepet çekmecesi)
│   ├── checkout/             # Checkout + sipariş onayı (checkout/[orderId])
│   ├── account/              # Hesabım (profil, siparişler, adresler, yorumlar, kargo — tab'lı)
│   ├── login/ register/ forgot-password/ reset-password/
│   ├── track-order/          # Misafir sipariş takibi
│   ├── api/shop-products/    # Sunucu taraflı aggregation route (ürün + fiyat tek istekte)
│   └── admin/                # Yönetim paneli (bkz. §4)
├── components/
│   ├── admin/                # Admin kabuğu ve modül bazlı bileşenler
│   ├── account/              # Hesabım sekmeleri
│   ├── ui/                   # shadcn primitive bileşenleri
│   └── ...                   # site-header, product-card, cart-provider, checkout-view, vb.
├── lib/
│   ├── api/                  # Tip güvenli API katmanı (client, config, auth, cart, catalog, orders, ...)
│   ├── auth-context.tsx      # Auth state (Context + useSyncExternalStore)
│   ├── anonymous-id.ts       # Misafir/anonim kullanıcı kimliği
│   └── store-catalog.ts, product-cache.ts, format.ts, ...
├── public/                   # Görseller, hero.mp4, ikonlar
├── components.json           # shadcn/ui konfigürasyonu
├── next.config.mjs
├── Dockerfile
└── .env / .env.local
```

`services/`, `store/`, `hooks/` veya `routes/` gibi ayrı klasörler yok — routing App Router'daki dosya yapısından, "servis" katmanı `lib/api/` altından geliyor.

## 3. Müşteri Vitrini

- **Ana sayfa** — hero video, kategori vitrini, kupon marquee
- **Mağaza / ürün listesi** (`/shop`) — kategori filtresi, arama, sıralama (öne çıkan/fiyat artan-azalan), staggered grid reveal animasyonu
- **Ürün detay** (`/product/[slug]`)
- **Sepet** (`/cart`) — sayfa + her yerden açılabilen slide-out çekmece
- **Checkout** (`/checkout`, `/checkout/[orderId]`) — sipariş onayı dahil
- **Auth** — login, register, forgot/reset password
- **Hesabım** (`/account`) — profil, siparişler, adresler, yorumlarım, kargo bilgileri (sekmeli)
- **Sipariş takibi** (`/track-order`) — misafir kullanıcılar için sipariş sorgulama

## 4. Admin Paneli (`/admin`)

`AdminShell` (`components/admin/admin-shell.tsx`) ile sarmalanır; sadece `isAdmin` olan kullanıcılar erişebilir (client-side guard — auth bootstrap tamamlandığında admin değilse `/login`'e yönlendirilir). Admin ikonu header'ın sağ ikon grubunda, sadece admin kullanıcılara görünür.

İçerdiği modüller:
- Dashboard
- Ürünler (liste, yeni, düzenle — attribute ve varyant yönetimi dahil)
- Kategoriler (ağaç yapısı yöneticisi)
- Stok (stok kalemleri, rezervasyonlar)
- Siparişler (liste + detay)
- Ödemeler (liste + detay)
- Kuponlar (liste, yeni, düzenle)
- Kargo firmaları + Gönderiler (liste, detay, zaman çizelgesi)
- Müşteriler (liste + detay)
- Ayarlar
- Bildirim günlüğü

## 5. Backend ile Entegrasyon

Base URL yapılandırması `lib/api/config.ts` içinde:

```ts
API_BASE_URL        = process.env.NEXT_PUBLIC_API_URL ?? "https://intern-api.kayas.dev"   // istemci (tarayıcı) istekleri
SERVER_API_BASE_URL = process.env.INTERNAL_API_URL ?? API_BASE_URL                          // SSR / route handler istekleri
```

`INTERNAL_API_URL`, Docker içi ağ üzerinden backend'e ulaşmak için kullanılır (örn. `http://api:8080`) — böylece SSR istekleri tek bir dış IP'den geliyormuş gibi görünüp backend'deki IP bazlı rate limit'e takılmaz.

Tip güvenli fetch sarmalayıcısı `lib/api/client.ts` (`apiFetch<T>()`) şunları yönetir:
- Bellekteki access token'ı `Authorization: Bearer` header'ına ekleme
- `credentials: "include"` (refresh token cookie'sinin gönderilmesi için)
- Token süresi dolmaya yakınsa veya 401 alındığında otomatik yenileme (eşzamanlı istekler için tek bir paylaşılan `refreshPromise`)
- Backend'in RFC 7807 `ProblemDetails` formatındaki hata gövdelerini `ApiError`'a çevirme

Domain bazlı API modülleri `lib/api/` altında: `auth`, `cart`, `catalog`, `customers`, `inventory`, `notifications`, `orders`, `payments`, `pricing`, `reviews`, `shipping`.

`app/api/shop-products/route.ts`, mağaza sayfasında istemci tarafından yapılacak çok sayıda küçük isteği tek bir sunucu taraflı istekte birleştirir (ürün + toplu fiyat) — backend rate limit sorunlarını azaltmak için eklendi.

## 6. Kimlik Doğrulama

- **Access token**: yalnızca `lib/api/token-store.ts` içindeki modül seviyesinde bir değişkende tutulur; **localStorage/sessionStorage'a asla yazılmaz**. `useSyncExternalStore` ile reaktif olarak `lib/auth-context.tsx`'e sunulur.
- **Refresh token**: backend tarafından set edilen HttpOnly cookie'de saklanır, JS tarafından hiç dokunulmaz. Sayfa yenilendiğinde `AuthProvider`, `POST /api/auth/refresh`'i `credentials: "include"` ile çağırarak oturumu geri kurar.
- Eski sürümde access token localStorage'da tutuluyordu; `token-store.ts` açılışta eski `vbt-access-token` anahtarını temizler (bir kerelik migration).
- Kullanıcı kimliği/rolü JWT payload'ından client-side decode edilir (`decodeJwt`).
- Rota koruması yalnızca client-side'dır — sunucu taraflı bir `middleware.ts` yoktur.
- Misafir kullanıcılar için `lib/anonymous-id.ts` bir anonim kimlik üretip saklar; bu kimlik misafir sepetini ve login/register sırasında sepet birleştirmeyi mümkün kılar.

## 7. Çalıştırma

```bash
npm install        # veya pnpm install
```

`.env.local` içine backend adresini yazın:

```
NEXT_PUBLIC_API_URL=https://your-backend-url
```

```bash
npm run dev         # geliştirme sunucusu
npm run build        # production build
npm run start        # production sunucusunu başlat
npm run lint         # eslint
```

> Not: Proje kökünde `.env.example` bulunmuyor; yukarıdaki `NEXT_PUBLIC_API_URL` (client'a gömülür) ve isteğe bağlı `INTERNAL_API_URL` (yalnızca sunucu tarafında, SSR/Docker-içi çağrılar için) değişkenlerini kendi `.env.local` dosyanıza eklemeniz gerekir.

### Docker ile

`frontend/Dockerfile` 3 aşamalı bir build tanımlar:
1. `deps` — `npm ci`
2. `builder` — `NEXT_PUBLIC_API_URL` build **ARG**'ı olarak alınır (client bundle'a build-time'da gömülür), `npm run build`
3. `runner` — Node 22-slim, `.next/standalone` çıktısı ile 3000 portunda `node server.js` çalıştırır

`backend/docker-compose.yml` içinde `frontend` servisi olarak da tanımlıdır (`INTERNAL_API_URL=http://api:8080` ile).

## 8. Test

Şu an herhangi bir test altyapısı (Jest/Vitest/Cypress/Playwright) yok, `package.json` içinde `test` script'i de bulunmuyor. Backend'in aksine frontend'de otomatik test kapsamı henüz eklenmedi.

## 9. Notlar

- Arayüz metinleri ve kod içi yorumlar Türkçe; hedef kitle Türkiye pazarı, para birimi ₺.
- Tailwind v4 kullanıldığı için ayrı bir `tailwind.config.*` dosyası yok — konfigürasyon `app/globals.css` içindeki `@theme` direktifiyle yönetiliyor.
- `next.config.mjs`: `output: "standalone"` (Docker için), `typescript.ignoreBuildErrors: true`, `images.unoptimized: true`.
