import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { CheckoutView } from "@/components/checkout-view"

export const metadata = { title: "Ödeme — Trendora" }

export default function CheckoutPage() {
  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <CheckoutView />
      <SiteFooter />
    </main>
  )
}
