import { getProductById } from "@/lib/api/catalog"
import { getPrice } from "@/lib/api/pricing"
import { ApiError } from "@/lib/api/client"
import type { CategoryTree, ProductListItem, SellableItemType } from "@/lib/api/types"

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
async function resolveListPrice(product: ProductListItem) {
  const productPrice = await getPriceOrNull("Product", product.id)
  if (productPrice !== null) return productPrice
  if (product.productType !== "Variant") return null

  const full = await getProductById(product.id).catch(() => null)
  const activeVariant = full?.variants.find((v) => v.isActive)
  if (!activeVariant) return null

  return getPriceOrNull("Variant", activeVariant.id)
}

export async function withListPrices(products: ProductListItem[]) {
  const prices = await Promise.all(products.map((p) => resolveListPrice(p)))
  return products.map((p, i) => ({ ...p, price: prices[i] }))
}

// Kart üzerinde hover ile diğer görselleri gezdirebilmek için ürünün (varyanttan bağımsız) tüm
// görsellerini de getirir — withListPrices'ın üstüne, sadece bu görseli isteyen yerler için.
export async function withListPricesAndImages(products: ProductListItem[]) {
  return Promise.all(
    products.map(async (p) => {
      const [price, full] = await Promise.all([resolveListPrice(p), getProductById(p.id).catch(() => null)])

      // Prefer images not tied to a specific variant; some products (e.g. every image attached to
      // a particular color/size) have none, so fall back to all of the product's images regardless
      // of variant rather than collapsing to a single image and losing the hover gallery entirely.
      const genericImages = full?.images.filter((img) => !img.productVariantId) ?? []
      const gallerySource = genericImages.length > 0 ? genericImages : (full?.images ?? [])
      const images = gallerySource.sort((a, b) => a.displayOrder - b.displayOrder).map((img) => img.url)

      return { ...p, price, images: images.length > 0 ? images : p.primaryImageUrl ? [p.primaryImageUrl] : [] }
    }),
  )
}

export function flattenCategories(tree: CategoryTree[]): CategoryTree[] {
  return tree.flatMap((node) => [node, ...flattenCategories(node.children)])
}

export function findCategoryName(tree: CategoryTree[], categoryId: string): string {
  const flat = flattenCategories(tree)
  return flat.find((c) => c.id === categoryId)?.name ?? "Kategori"
}
