import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { CartView } from "@/components/cart-view"

export const metadata = {
  title: "Sepet — Trendora",
}

export default function CartPage() {
  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <CartView />
      <SiteFooter />
    </main>
  )
}
