"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import * as shippingApi from "@/lib/api/shipping"
import * as myOrdersApi from "@/lib/api/orders"
import type { Order } from "@/lib/api/types"
import { formatDate, formatPrice } from "@/lib/format"
import { orderStatusLabels, orderStatusTone } from "@/lib/enum-labels"
import { Badge } from "@/components/ui/badge"
import { Pagination } from "@/components/ui/pagination"
import { EmptyState } from "@/components/ui/empty-state"
import { PageSpinner } from "@/components/ui/spinner"

const PAGE_SIZE = 5

export function OrdersTab() {
  const [orders, setOrders] = useState<Order[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)
  const [companyNames, setCompanyNames] = useState<Record<string, string>>({})

  async function load(page: number) {
    setLoading(true)
    try {
      const result = await myOrdersApi.getMyOrders(page, PAGE_SIZE)
      setOrders(result.items)
      setTotalPages(result.totalPages || 1)

      const companies = await shippingApi.getActiveShippingCompanies().catch(() => [])
      setCompanyNames(Object.fromEntries(companies.map((c) => [c.id, c.name])))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load(pageNumber)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber])

  async function handleCancel(orderId: string) {
    if (!window.confirm("Bu siparişi iptal etmek istediğinize emin misiniz?")) return
    await myOrdersApi.cancelMyOrder(orderId)
    await load(pageNumber)
  }

  if (loading) return <PageSpinner label="Siparişler yükleniyor…" />

  if (orders.length === 0) {
    return (
      <EmptyState
        title="Henüz siparişiniz yok"
        description="Alışverişe başladığınızda siparişleriniz burada listelenecek."
        action={
          <Link href="/shop" className="border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background">
            Alışverişe Başla
          </Link>
        }
      />
    )
  }

  return (
    <section className="space-y-6" aria-label="Siparişlerim">
      {orders.map((order) => (
        <article key={order.id} className="border border-border">
          <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-5 py-4">
            <div>
              <p className="text-sm font-semibold tracking-tight">{order.id.slice(0, 8).toUpperCase()}</p>
              <p className="text-xs text-muted-foreground">{formatDate(order.createdAt)}</p>
            </div>
            <Badge tone={orderStatusTone[order.status]}>{orderStatusLabels[order.status]}</Badge>
          </div>
          <ul className="divide-y divide-border">
            {order.items.map((item, i) => (
              <li key={i} className="flex items-center justify-between px-5 py-3 text-sm">
                <span className="text-muted-foreground">{item.quantity} adet</span>
                <span className="tabular-nums">{formatPrice(item.lineSubtotal)}</span>
              </li>
            ))}
          </ul>
          <div className="flex flex-wrap items-center justify-between gap-3 border-t border-border px-5 py-4">
            <div className="text-xs text-muted-foreground">
              Kargo: {companyNames[order.shippingCompanyId] ?? "—"} · {formatPrice(order.shippingFee)}
            </div>
            <span className="text-sm font-semibold tabular-nums">{formatPrice(order.grandTotal)}</span>
          </div>
          <div className="flex items-center gap-4 border-t border-border px-5 py-3 text-xs font-medium uppercase tracking-widest">
            <Link href={`/checkout/${order.id}`} className="text-muted-foreground hover:text-foreground">
              Detayları Gör
            </Link>
            {order.status === "Pending" && (
              <button type="button" onClick={() => handleCancel(order.id)} className="text-muted-foreground hover:text-foreground">
                Siparişi İptal Et
              </button>
            )}
          </div>
        </article>
      ))}
      <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
    </section>
  )
}
