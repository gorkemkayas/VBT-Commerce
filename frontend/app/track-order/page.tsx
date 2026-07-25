import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { TrackOrderForm } from "@/components/track-order-form"

export const metadata = { title: "Sipariş Sorgula — Trendora" }

export default function TrackOrderPage() {
  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <TrackOrderForm />
      <SiteFooter />
    </main>
  )
}
