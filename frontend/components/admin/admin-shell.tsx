"use client"

import { useEffect } from "react"
import Link from "next/link"
import { usePathname, useRouter } from "next/navigation"
import {
  Bell,
  Boxes,
  CreditCard,
  LayoutGrid,
  LogOut,
  Package,
  Percent,
  Settings,
  ShoppingCart,
  Store,
  Tag,
  Truck,
  Users,
} from "lucide-react"
import { useAuth } from "@/lib/auth-context"
import { PageSpinner } from "@/components/ui/spinner"

const navGroups: { title: string; items: { label: string; href: string; icon: typeof Package }[] }[] = [
  {
    title: "Katalog",
    items: [
      { label: "Ürünler", href: "/admin/products", icon: Package },
      { label: "Kategoriler", href: "/admin/categories", icon: LayoutGrid },
    ],
  },
  {
    title: "Stok",
    items: [
      { label: "Stok Kalemleri", href: "/admin/inventory", icon: Boxes },
      { label: "Rezervasyonlar", href: "/admin/inventory/reservations", icon: Boxes },
    ],
  },
  {
    title: "Satış",
    items: [
      { label: "Siparişler", href: "/admin/orders", icon: ShoppingCart },
      { label: "Ödemeler", href: "/admin/payments", icon: CreditCard },
      { label: "Kuponlar", href: "/admin/coupons", icon: Tag },
    ],
  },
  {
    title: "Kargo",
    items: [
      { label: "Kargo Firmaları", href: "/admin/shipping-companies", icon: Truck },
      { label: "Gönderiler", href: "/admin/shipments", icon: Truck },
    ],
  },
  {
    title: "Yönetim",
    items: [
      { label: "Müşteriler", href: "/admin/customers", icon: Users },
      { label: "Ayarlar", href: "/admin/settings", icon: Settings },
      { label: "Bildirim Günlüğü", href: "/admin/notifications", icon: Bell },
    ],
  },
]

export function AdminShell({ children }: { children: React.ReactNode }) {
  const router = useRouter()
  const pathname = usePathname()
  const { isAdmin, bootstrapping, user, logout } = useAuth()

  useEffect(() => {
    if (!bootstrapping && !isAdmin) {
      // Düz "/login" — buraya "?redirect=/admin" eklenmiyor, aksi halde admin panelinden
      // çıkış yapıp müşteri hesabıyla giriş yapmaya çalışan biri her seferinde admin'e geri
      // (ve dolayısıyla tekrar login'e) fırlatılır.
      router.replace("/login")
    }
  }, [bootstrapping, isAdmin, router])

  if (bootstrapping || !isAdmin) return <PageSpinner label="Yönetim paneli yükleniyor…" />

  return (
    <div className="flex min-h-screen bg-secondary/40 text-foreground">
      <aside className="hidden w-64 shrink-0 flex-col border-r border-border bg-background md:flex">
        <div className="flex h-16 items-center gap-2.5 border-b border-border px-6">
          <span className="font-brand text-xl font-normal tracking-[0.15em]">Trendora</span>
          <span className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Admin</span>
        </div>
        <nav className="flex-1 overflow-y-auto px-3 py-6">
          {navGroups.map((group, groupIndex) => (
            <div key={group.title} className={groupIndex > 0 ? "mt-6" : undefined}>
              <p className="px-3 pb-2 text-[10px] font-semibold uppercase tracking-widest text-muted-foreground/80">{group.title}</p>
              <div className="space-y-0.5">
                {group.items.map((item) => {
                  const active = pathname === item.href || pathname.startsWith(item.href + "/")
                  return (
                    <Link
                      key={item.href}
                      href={item.href}
                      className={`flex items-center gap-2.5 border-l-2 px-3 py-2 text-sm transition-colors ${
                        active
                          ? "border-foreground bg-foreground/[0.05] font-medium text-foreground"
                          : "border-transparent text-foreground/70 hover:border-border hover:bg-secondary hover:text-foreground"
                      }`}
                    >
                      <item.icon className="h-4 w-4 shrink-0" strokeWidth={1.5} />
                      {item.label}
                    </Link>
                  )
                })}
              </div>
            </div>
          ))}
        </nav>
        <div className="border-t border-border p-4">
          <Link href="/" className="flex items-center gap-2 text-xs text-muted-foreground transition-colors hover:text-foreground">
            <Store className="h-3.5 w-3.5" strokeWidth={1.5} />
            Mağazaya Dön
          </Link>
        </div>
      </aside>

      <div className="flex-1">
        <header className="flex h-16 items-center justify-between border-b border-border bg-background px-4 md:px-8">
          <span className="font-brand text-lg font-normal tracking-[0.1em] md:hidden">Trendora Admin</span>
          <div className="hidden md:block" />
          <div className="flex items-center gap-4">
            <span className="border border-border px-2 py-1 text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Admin</span>
            <span className="text-xs text-muted-foreground">{user?.email}</span>
            <button
              type="button"
              onClick={() => logout()}
              aria-label="Çıkış yap"
              className="text-muted-foreground transition-colors hover:text-foreground"
            >
              <LogOut className="h-4 w-4" strokeWidth={1.5} />
            </button>
          </div>
        </header>
        <main className="p-4 md:p-8">{children}</main>
      </div>
    </div>
  )
}
