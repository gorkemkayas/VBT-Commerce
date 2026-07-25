import { SiteHeader } from "@/components/site-header"
import { HeroSection } from "@/components/hero-section"
import { CouponMarquee } from "@/components/coupon-marquee"
import { CategoryShowcase } from "@/components/category-showcase"
import { ProductGrid } from "@/components/product-grid"
import { SiteFooter } from "@/components/site-footer"

export const dynamic = "force-dynamic"

export default function Page() {
  return (
    <div className="min-h-screen bg-background">
      <SiteHeader overlay />

      <main>
        <HeroSection />
        <CouponMarquee />
        <CategoryShowcase />
        <ProductGrid />
      </main>

      <SiteFooter />
    </div>
  )
}
