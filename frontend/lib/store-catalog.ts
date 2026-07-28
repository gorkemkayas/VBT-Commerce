import { getProductsByIds } from "@/lib/api/catalog"
import { getPrice, getPrices } from "@/lib/api/pricing"
import { ApiError } from "@/lib/api/client"
import type { CategoryTree, Product, ProductListItem, SellableItemType } from "@/lib/api/types"

export async function getPriceOrNull(sellableItemType: SellableItemType, sellableItemId: string) {
  try {
    const price = await getPrice(sellableItemType, sellableItemId)
    return price.amount
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null
    throw error
  }
}

// Variant tipi ürünlerin fiyatı ürünün kendisinde değil, varyantlarında tutulur — ürün seviyesinde
// fiyat bulunamazsa (yaygın durum), listelemede göstermek için ilk aktif varyantın fiyatına düşer.
//
// Fiyatlar VE varyant/görsel keşfi, liste boyutundan bağımsız olarak sabit sayıda toplu (batch)
// istekle çözülür: ürün-seviyesi fiyatlar tek çağrıda, fiyatı bulunamayan Variant tipi ürünlerin tam
// detayları (aktif varyantı bulmak için) tek çağrıda, o varyantların fiyatları yine tek bir toplu
// çağrıyla gelir. `fullProductsById` verilmişse (withListPricesAndImages görseller için zaten tüm
// ürün detaylarını çekmiş oluyor) o veri yeniden kullanılır, ekstra istek atılmaz.
async function resolveListPricesBatch(products: ProductListItem[], fullProductsById?: Map<string, Product>) {
  const productPrices = await getPrices(products.map((p) => ({ sellableItemId: p.id, sellableItemType: "Product" as const })))
  const priceByProductId = new Map(productPrices.map((p) => [p.sellableItemId, p.amount]))

  const needsVariant = products.filter((p) => !priceByProductId.has(p.id) && p.productType === "Variant")

  const fullByProductId = fullProductsById ?? new Map((await getProductsByIds(needsVariant.map((p) => p.id))).map((p) => [p.id, p]))

  const activeVariantByProductId = new Map<string, string>()
  for (const p of needsVariant) {
    const activeVariant = fullByProductId.get(p.id)?.variants.find((v) => v.isActive)
    if (activeVariant) activeVariantByProductId.set(p.id, activeVariant.id)
  }

  const variantIds = [...activeVariantByProductId.values()]
  const variantPrices = await getPrices(variantIds.map((id) => ({ sellableItemId: id, sellableItemType: "Variant" as const })))
  const priceByVariantId = new Map(variantPrices.map((p) => [p.sellableItemId, p.amount]))

  return new Map(
    products.map((p) => {
      const activeVariantId = activeVariantByProductId.get(p.id)
      const price = priceByProductId.get(p.id) ?? (activeVariantId ? (priceByVariantId.get(activeVariantId) ?? null) : null)
      return [p.id, price] as const
    }),
  )
}

export async function withListPrices(products: ProductListItem[]) {
  const priceById = await resolveListPricesBatch(products)
  return products.map((p) => ({ ...p, price: priceById.get(p.id) ?? null }))
}

// Kart üzerinde hover ile diğer görselleri gezdirebilmek için ürünün (varyanttan bağımsız) tüm
// görsellerini de getirir — withListPrices'ın üstüne, sadece bu görseli isteyen yerler için.
export async function withListPricesAndImages(products: ProductListItem[]) {
  const fullProductsById = new Map((await getProductsByIds(products.map((p) => p.id))).map((p) => [p.id, p]))

  const priceById = await resolveListPricesBatch(products, fullProductsById)

  return products.map((p) => {
    const full = fullProductsById.get(p.id)

    // Prefer images not tied to a specific variant; some products (e.g. every image attached to
    // a particular color/size) have none, so fall back to all of the product's images regardless
    // of variant rather than collapsing to a single image and losing the hover gallery entirely.
    const genericImages = full?.images.filter((img) => !img.productVariantId) ?? []
    const gallerySource = genericImages.length > 0 ? genericImages : (full?.images ?? [])
    const images = gallerySource.sort((a, b) => a.displayOrder - b.displayOrder).map((img) => img.url)

    return { ...p, price: priceById.get(p.id) ?? null, images: images.length > 0 ? images : p.primaryImageUrl ? [p.primaryImageUrl] : [] }
  })
}

export function flattenCategories(tree: CategoryTree[]): CategoryTree[] {
  return tree.flatMap((node) => [node, ...flattenCategories(node.children)])
}

export function findCategoryName(tree: CategoryTree[], categoryId: string): string {
  const flat = flattenCategories(tree)
  return flat.find((c) => c.id === categoryId)?.name ?? "Kategori"
}
