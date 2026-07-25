"use client"

import { useEffect, useState } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import { MapPin, MessageSquare, Package, Truck, User } from "lucide-react"
import { useAuth } from "@/lib/auth-context"
import { OrdersTab } from "@/components/account/orders-tab"
import { ShippingTab } from "@/components/account/shipping-tab"
import { ProfileTab } from "@/components/account/profile-tab"
import { AddressesTab } from "@/components/account/addresses-tab"
import { ReviewsTab } from "@/components/account/reviews-tab"
import { PageSpinner } from "@/components/ui/spinner"

type Tab = "orders" | "shipping" | "info" | "addresses" | "reviews"

const tabs: { id: Tab; label: string; icon: typeof Package }[] = [
  { id: "orders", label: "Siparişlerim", icon: Package },
  { id: "shipping", label: "Kargo Takibi", icon: Truck },
  { id: "info", label: "Kişisel Bilgiler", icon: User },
  { id: "addresses", label: "Adreslerim", icon: MapPin },
  { id: "reviews", label: "Yorumlarım", icon: MessageSquare },
]

export function ProfileView() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const { isAuthenticated, bootstrapping, user, logout } = useAuth()

  const requestedTab = searchParams.get("tab")
  const [tab, setTab] = useState<Tab>(tabs.some((t) => t.id === requestedTab) ? (requestedTab as Tab) : "orders")

  useEffect(() => {
    if (!bootstrapping && !isAuthenticated) {
      router.replace("/login?redirect=/account")
    }
  }, [bootstrapping, isAuthenticated, router])

  if (bootstrapping || !isAuthenticated) return <PageSpinner label="Hesap bilgileri yükleniyor…" />

  return (
    <div className="mx-auto max-w-7xl px-4 py-10 md:px-6 md:py-16">
      <header className="mb-10 flex flex-wrap items-end justify-between gap-4 border-b border-border pb-8">
        <div>
          <p className="text-xs uppercase tracking-widest text-muted-foreground">Hesabım</p>
          <h1 className="mt-2 font-serif text-3xl font-medium tracking-tight md:text-4xl">
            {user?.firstName || user?.lastName ? `${user.firstName} ${user.lastName}`.trim() : user?.email}
          </h1>
        </div>
        <button
          type="button"
          onClick={() => logout()}
          className="text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground"
        >
          Çıkış Yap
        </button>
      </header>

      <div className="grid gap-10 md:grid-cols-[220px_1fr]">
        <nav className="flex gap-2 overflow-x-auto md:flex-col md:gap-0" aria-label="Hesap bölümleri">
          {tabs.map(({ id, label, icon: Icon }) => (
            <button
              key={id}
              type="button"
              onClick={() => setTab(id)}
              className={`flex shrink-0 items-center gap-3 border px-4 py-3 text-left text-sm font-medium transition-colors md:border-x-0 md:border-t-0 md:border-b md:border-border ${
                tab === id
                  ? "border-foreground bg-foreground text-background md:bg-transparent md:text-foreground"
                  : "border-border text-muted-foreground hover:text-foreground"
              }`}
            >
              <Icon className="h-4 w-4" strokeWidth={1.5} />
              {label}
            </button>
          ))}
        </nav>

        <div>
          {tab === "orders" && <OrdersTab />}
          {tab === "shipping" && <ShippingTab />}
          {tab === "info" && <ProfileTab />}
          {tab === "addresses" && <AddressesTab />}
          {tab === "reviews" && <ReviewsTab />}
        </div>
      </div>
    </div>
  )
}
