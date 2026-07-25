import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { SideRails } from "@/components/side-rails"
import { ShopView } from "@/components/shop-view"
import { getCategoryTree } from "@/lib/api/catalog"

export const dynamic = "force-dynamic"

export default async function ShopPage({
  searchParams,
}: {
  searchParams: Promise<{ categoryId?: string }>
}) {
  const { categoryId } = await searchParams
  const categories = await getCategoryTree()

  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <div className="relative">
        <SideRails />
        <ShopView categories={categories} initialCategoryId={categoryId} />
      </div>
      <SiteFooter />
    </main>
  )
}
