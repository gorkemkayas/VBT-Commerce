import * as catalogApi from "@/lib/api/catalog"
import type { ProductListItem, ProductVariant } from "@/lib/api/types"

export type CatalogEntry = {
  productId: string
  productName: string
  productImage: string | null
  variant: ProductVariant | null
  variantImage: string | null
}

export function variantLabel(variant: ProductVariant) {
  return variant.optionValues.length > 0 ? variant.optionValues.map((o) => o.value).join(" / ") : variant.sku
}

// Backend, stok kalemi/rezervasyon id'sini ürün/varyant adına çeviren bir endpoint sunmuyor; bu yüzden
// tüm katalog burada tek seferde çekilip sellableItemId -> ürün/varyant eşlemesi kuruluyor.
export async function buildCatalogIndex(): Promise<Map<string, CatalogEntry>> {
  const index = new Map<string, CatalogEntry>()
  const allProducts: ProductListItem[] = []
  let pageNumber = 1
  let totalPages = 1
  do {
    const result = await catalogApi.getProducts({ pageNumber, pageSize: 100 })
    allProducts.push(...result.items)
    totalPages = result.totalPages || 1
    pageNumber++
  } while (pageNumber <= totalPages)

  for (const product of allProducts) {
    if (product.productType === "Simple") {
      index.set(product.id, {
        productId: product.id,
        productName: product.name,
        productImage: product.primaryImageUrl,
        variant: null,
        variantImage: null,
      })
    }
  }

  const variantProducts = allProducts.filter((p) => p.productType === "Variant")
  const details = await Promise.all(variantProducts.map((p) => catalogApi.getProductById(p.id).catch(() => null)))

  details.forEach((detail, i) => {
    if (!detail) return
    const listItem = variantProducts[i]
    for (const variant of detail.variants) {
      const variantImage =
        detail.images.find((img) => img.productVariantId === variant.id)?.url ??
        detail.images.find((img) => !img.productVariantId)?.url ??
        listItem.primaryImageUrl ??
        null
      index.set(variant.id, {
        productId: detail.id,
        productName: detail.name,
        productImage: listItem.primaryImageUrl,
        variant,
        variantImage,
      })
    }
  })

  return index
}
