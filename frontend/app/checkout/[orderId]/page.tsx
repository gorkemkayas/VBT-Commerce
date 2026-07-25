import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { OrderConfirmation } from "@/components/order-confirmation"

export const dynamic = "force-dynamic"

export const metadata = { title: "Sipariş Onayı — Trendora" }

export default async function OrderConfirmationPage({ params }: { params: Promise<{ orderId: string }> }) {
  const { orderId } = await params
  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <OrderConfirmation orderId={orderId} />
      <SiteFooter />
    </main>
  )
}
