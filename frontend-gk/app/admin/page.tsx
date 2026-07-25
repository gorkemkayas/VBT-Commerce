import Link from "next/link"
import { Boxes, CreditCard, LayoutGrid, Package, ShoppingCart, Tag, Truck, Users } from "lucide-react"
import { AdminPageHeader } from "@/components/admin/page-header"

const shortcuts = [
  { label: "Ürünler", href: "/admin/products", icon: Package, desc: "Katalog yönetimi" },
  { label: "Kategoriler", href: "/admin/categories", icon: LayoutGrid, desc: "Kategori ağacı" },
  { label: "Stok", href: "/admin/inventory", icon: Boxes, desc: "Stok seviyeleri" },
  { label: "Siparişler", href: "/admin/orders", icon: ShoppingCart, desc: "Tüm siparişler" },
  { label: "Ödemeler", href: "/admin/payments", icon: CreditCard, desc: "Ödeme kayıtları" },
  { label: "Kuponlar", href: "/admin/coupons", icon: Tag, desc: "İndirim kodları" },
  { label: "Kargo Firmaları", href: "/admin/shipping-companies", icon: Truck, desc: "Kargo & gönderiler" },
  { label: "Müşteriler", href: "/admin/customers", icon: Users, desc: "Müşteri listesi" },
]

export default function AdminDashboardPage() {
  return (
    <div>
      <AdminPageHeader title="Genel Bakış" description="Yönetim paneline hoş geldiniz." />
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {shortcuts.map((s) => (
          <Link
            key={s.href}
            href={s.href}
            className="group border border-border bg-background p-5 shadow-[0_1px_2px_rgba(0,0,0,0.03)] transition-all hover:-translate-y-0.5 hover:border-foreground/40 hover:shadow-[0_6px_16px_rgba(0,0,0,0.06)]"
          >
            <s.icon className="h-5 w-5 text-muted-foreground transition-colors group-hover:text-foreground" strokeWidth={1.5} />
            <p className="mt-3 text-sm font-medium">{s.label}</p>
            <p className="mt-1 text-xs text-muted-foreground">{s.desc}</p>
          </Link>
        ))}
      </div>
    </div>
  )
}
