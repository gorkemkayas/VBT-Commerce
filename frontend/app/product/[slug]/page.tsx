import Link from "next/link"
import { notFound } from "next/navigation"
import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { CouponMarquee } from "@/components/coupon-marquee"
import { SideRails } from "@/components/side-rails"
import { ProductDetail } from "@/components/product-detail"
import { ProductCard } from "@/components/product-card"
import { getCategoryTree, getProductBySlug, getProducts } from "@/lib/api/catalog"
import { getProductReviewSummary, getProductReviews } from "@/lib/api/reviews"
import { ApiError } from "@/lib/api/client"
import { findCategoryName, withListPrices } from "@/lib/store-catalog"

export const dynamic = "force-dynamic"

export async function generateMetadata({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params
  try {
    const product = await getProductBySlug(slug)
    return { title: `${product.name} — Trendora`, description: product.description ?? undefined }
  } catch {
    return { title: "Ürün bulunamadı — Trendora" }
  }
}

export default async function ProductPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params

  let product
  try {
    product = await getProductBySlug(slug)
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) notFound()
    throw error
  }

  const [categoryTree, reviewsPage, summary, sameCategoryPage, fallbackPage] = await Promise.all([
    getCategoryTree(),
    getProductReviews(product.id, "Product", 1, 10),
    getProductReviewSummary(product.id, "Product"),
    getProducts({ pageNumber: 1, pageSize: 8, categoryId: product.categoryId, isActive: true }),
    getProducts({ pageNumber: 1, pageSize: 8, isActive: true }),
  ])

  const categoryName = findCategoryName(categoryTree, product.categoryId)

  // Aynı kategoride yeterli ürün yoksa (çoğu kategoride 1-2 ürün var), listeyi diğer aktif
  // ürünlerle tamamlıyoruz — böylece bölüm neredeyse hiç boş kalmıyor.
  const seen = new Set([product.id])
  const relatedItems = [...sameCategoryPage.items, ...fallbackPage.items].filter((p) => {
    if (seen.has(p.id)) return false
    seen.add(p.id)
    return true
  })
  const related = await withListPrices(relatedItems.slice(0, 4))

  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <CouponMarquee />
      <div className="relative">
        <SideRails />
        <ProductDetail product={product} categoryName={categoryName} initialReviews={reviewsPage.items} initialSummary={summary} />

        {related.length > 0 && (
          <section className="mt-20 border-t border-border pb-16">
            <div className="mx-auto flex max-w-7xl items-end justify-between px-6 py-8 md:px-10">
              <h2 className="font-serif text-3xl font-medium tracking-tight md:text-4xl">Benzer Ürünler</h2>
              <Link href="/shop" className="text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground">
                Tümünü Gör
              </Link>
            </div>
            <div className="mx-auto grid max-w-7xl grid-cols-2 gap-x-6 gap-y-10 px-6 md:px-10 lg:grid-cols-4 lg:gap-x-8">
              {related.map((p) => (
                <ProductCard
                  key={p.id}
                  product={{
                    id: p.id,
                    slug: p.slug,
                    name: p.name,
                    image: p.primaryImageUrl,
                    price: p.price,
                    categoryName: findCategoryName(categoryTree, p.categoryId),
                  }}
                />
              ))}
            </div>
          </section>
        )}
      </div>

      <SiteFooter />
    </main>
  )
}
