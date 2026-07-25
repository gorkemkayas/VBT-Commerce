// Sepet/sipariş satırları backend'den sadece sellableItemId + quantity olarak gelir; ürün adı,
// görseli ve birim fiyatı yok. Bu küçük önbellek, ürün/varyant sayfaları gezilirken doldurulur ve
// sepet/sipariş ekranlarında satırları isim+görselle zenginleştirmek için kullanılır.

export type CachedSellableItem = {
  id: string
  productId: string
  productName: string
  productSlug: string
  image: string | null
  variantLabel: string | null
  unitPrice: number | null
}

const STORAGE_KEY = "vbt-product-cache"

function readAll(): Record<string, CachedSellableItem> {
  if (typeof window === "undefined") return {}
  try {
    const raw = window.localStorage.getItem(STORAGE_KEY)
    return raw ? JSON.parse(raw) : {}
  } catch {
    return {}
  }
}

function writeAll(map: Record<string, CachedSellableItem>) {
  if (typeof window === "undefined") return
  try {
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(map))
  } catch {
    // yoksay
  }
}

export function cacheSellableItems(items: CachedSellableItem[]) {
  if (!items.length) return
  const all = readAll()
  for (const item of items) all[item.id] = item
  writeAll(all)
}

export function getCachedSellableItem(id: string): CachedSellableItem | null {
  return readAll()[id] ?? null
}
