"use client"

import { useEffect, useState } from "react"
import { ChevronDown } from "lucide-react"
import * as myOrdersApi from "@/lib/api/orders"
import * as shippingApi from "@/lib/api/shipping"
import type { Order, ShipmentTracking } from "@/lib/api/types"
import { formatDate, formatPrice } from "@/lib/format"
import { shipmentStatusLabels, shipmentStatusTone } from "@/lib/enum-labels"
import { Badge } from "@/components/ui/badge"
import { Pagination } from "@/components/ui/pagination"
import { EmptyState } from "@/components/ui/empty-state"
import { PageSpinner } from "@/components/ui/spinner"
import { ShipmentTimeline } from "@/components/shipment-timeline"

const PAGE_SIZE = 5

export function ShippingTab() {
  const [orders, setOrders] = useState<Order[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [companyNames, setCompanyNames] = useState<Record<string, string>>({})
  const [shipments, setShipments] = useState<Record<string, ShipmentTracking | null>>({})
  const [expanded, setExpanded] = useState<Record<string, boolean>>({})
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    ;(async () => {
      setLoading(true)
      try {
        const [result, companies] = await Promise.all([
          myOrdersApi.getMyOrders(pageNumber, PAGE_SIZE),
          shippingApi.getActiveShippingCompanies().catch(() => []),
        ])
        const activeOrders = result.items.filter((o) => o.status !== "Cancelled")
        setOrders(activeOrders)
        setTotalPages(result.totalPages || 1)
        setCompanyNames(Object.fromEntries(companies.map((c) => [c.id, c.name])))

        const shipmentEntries = await Promise.all(
          activeOrders.map(async (order) => {
            const shipment = await myOrdersApi.getMyOrderShipment(order.id).catch(() => null)
            return [order.id, shipment] as const
          }),
        )
        setShipments(Object.fromEntries(shipmentEntries))
      } finally {
        setLoading(false)
      }
    })()
  }, [pageNumber])

  function toggleExpanded(orderId: string) {
    setExpanded((prev) => ({ ...prev, [orderId]: !prev[orderId] }))
  }

  if (loading) return <PageSpinner label="Kargo bilgisi yükleniyor…" />

  if (orders.length === 0) {
    return <EmptyState title="Şu anda kargo sürecinde ürününüz bulunmuyor." />
  }

  return (
    <section className="space-y-6" aria-label="Kargo takibi">
      {orders.map((order) => {
        const shipment = shipments[order.id]
        const isOpen = !!expanded[order.id]
        return (
          <article key={order.id} className="border border-border">
            <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-5 py-4">
              <div>
                <p className="text-sm font-semibold tracking-tight">{order.id.slice(0, 8).toUpperCase()}</p>
                <p className="text-xs text-muted-foreground">
                  {companyNames[order.shippingCompanyId] ?? "Kargo firması"} · {formatPrice(order.shippingFee)}
                </p>
              </div>
              {shipment && <Badge tone={shipmentStatusTone[shipment.status]}>{shipmentStatusLabels[shipment.status]}</Badge>}
            </div>
            <div className="px-5 py-4 text-sm text-muted-foreground">
              <p>Sipariş tarihi: {formatDate(order.createdAt)}</p>
              <p className="mt-1">
                {order.addressLine1}, {order.district}/{order.city}
              </p>
              {shipment?.trackingNumber && <p className="mt-1">Takip No: {shipment.trackingNumber}</p>}
            </div>
            <button
              type="button"
              onClick={() => toggleExpanded(order.id)}
              className="flex w-full items-center justify-between border-t border-border px-5 py-3 text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground"
            >
              Detay
              <ChevronDown className={`h-3.5 w-3.5 transition-transform duration-300 ${isOpen ? "rotate-180" : ""}`} strokeWidth={1.5} />
            </button>
            <div className={`grid transition-all duration-300 ease-in-out ${isOpen ? "grid-rows-[1fr]" : "grid-rows-[0fr]"}`}>
              <div className="overflow-hidden">
                {shipment && shipment.history.length > 0 ? (
                  <div className="border-t border-border px-5 py-4">
                    <ShipmentTimeline history={shipment.history} />
                  </div>
                ) : (
                  <p className="border-t border-border px-5 py-4 text-xs text-muted-foreground">
                    Bu sipariş için kargo süreci bilgisi henüz oluşturulmadı.
                  </p>
                )}
              </div>
            </div>
          </article>
        )
      })}
      <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
    </section>
  )
}
