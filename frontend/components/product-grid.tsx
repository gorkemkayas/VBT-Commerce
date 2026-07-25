import Link from "next/link"
import { ProductCard } from "@/components/product-card"
import { getCategoryTree, getProducts } from "@/lib/api/catalog"
import { findCategoryName, withListPricesAndImages } from "@/lib/store-catalog"

export async function ProductGrid() {
  const [{ items }, categoryTree] = await Promise.all([
    getProducts({ pageNumber: 1, pageSize: 8, isActive: true }),
    getCategoryTree(),
  ])
  const products = await withListPricesAndImages(items)

  if (products.length === 0) return null

  return (
    <section id="koleksiyon" className="border-b border-border">
      <div className="flex items-end justify-between px-6 py-8 md:px-10">
        <h2 className="font-serif text-3xl font-medium tracking-tight md:text-4xl">Koleksiyon</h2>
        <Link href="/shop" className="text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground">
          Tümünü Gör
        </Link>
      </div>

      <div className="grid grid-cols-2 gap-x-6 gap-y-10 px-6 pb-16 md:px-10 lg:grid-cols-4 lg:gap-x-8">
        {products.map((product) => (
          <ProductCard
            key={product.id}
            product={{
              id: product.id,
              slug: product.slug,
              name: product.name,
              image: product.primaryImageUrl,
              images: product.images,
              price: product.price,
              categoryName: findCategoryName(categoryTree, product.categoryId),
            }}
          />
        ))}
      </div>
    </section>
  )
}
