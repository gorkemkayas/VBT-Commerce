"use client"

import { useCallback, useEffect, useState } from "react"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import * as ordersApi from "@/lib/api/orders"
import type { Order } from "@/lib/api/types"
import { formatDateTime, formatPrice } from "@/lib/format"
import { orderStatusLabels, orderStatusTone } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Badge } from "@/components/ui/badge"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

export function OrderDetail({ orderId }: { orderId: string }) {
  const [order, setOrder] = useState<Order | null>(null)
  const [loading, setLoading] = useState(true)
  const [notFound, setNotFound] = useState(false)

  const load = useCallback(async () => {
    try {
      setOrder(await ordersApi.getAdminOrder(orderId))
    } catch {
      setNotFound(true)
    } finally {
      setLoading(false)
    }
  }, [orderId])

  useEffect(() => {
    load()
  }, [load])

  async function handleCancel() {
    const reason = window.prompt("İptal sebebi (opsiyonel):", "")
    if (reason === null) return
    await ordersApi.cancelAdminOrder(orderId, reason || null)
    await load()
  }

  if (loading) return <PageSpinner label="Sipariş yükleniyor…" />
  if (notFound || !order) return <EmptyState title="Sipariş bulunamadı" />

  return (
    <div>
      <Link href="/admin/orders" className="mb-4 flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
        <ChevronLeft className="h-3.5 w-3.5" strokeWidth={1.5} />
        Siparişlere Dön
      </Link>
      <AdminPageHeader
        title={`Sipariş ${order.id.slice(0, 8).toUpperCase()}`}
        description={formatDateTime(order.createdAt)}
        action={
          <div className="flex items-center gap-3">
            <Badge tone={orderStatusTone[order.status]}>{orderStatusLabels[order.status]}</Badge>
            {order.status !== "Cancelled" && (
              <button type="button" onClick={handleCancel} className="border border-red-600/40 px-4 py-2 text-xs font-medium uppercase tracking-widest text-red-600 hover:bg-red-600/10">
                Siparişi İptal Et
              </button>
            )}
          </div>
        }
      />

      {order.cancelledReason && (
        <AdminSection title="İptal Sebebi">
          <p className="text-sm text-muted-foreground">{order.cancelledReason}</p>
        </AdminSection>
      )}

      <div className="mt-6 grid grid-cols-1 gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2 space-y-6">
          <AdminSection title="Kalemler">
            <ul className="divide-y divide-border">
              {order.items.map((item, i) => (
                <li key={i} className="flex items-center justify-between py-3 text-sm">
                  <span className="text-muted-foreground">
                    {item.sellableItemType === "Product" ? "Ürün" : "Varyant"} · {item.sellableItemId.slice(0, 8)} · {item.quantity} adet ×{" "}
                    {formatPrice(item.unitPrice)}
                  </span>
                  <span className="font-mono">{formatPrice(item.lineSubtotal)}</span>
                </li>
              ))}
            </ul>
          </AdminSection>

          <AdminSection title="Teslimat Adresi">
            <p className="text-sm">{order.recipientName}</p>
            <p className="text-sm text-muted-foreground">{order.phoneNumber}</p>
            <p className="mt-2 text-sm text-muted-foreground">
              {order.addressLine1}
              {order.addressLine2 ? `, ${order.addressLine2}` : ""}
            </p>
            <p className="text-sm text-muted-foreground">
              {order.district}/{order.city}, {order.postalCode} — {order.country}
            </p>
          </AdminSection>
        </div>

        <div className="space-y-6">
          <AdminSection title="Tutar Özeti">
            <dl className="space-y-2 text-sm">
              <Row label="Ara Toplam" value={formatPrice(order.subtotal)} />
              {order.discountAmount > 0 && <Row label="İndirim" value={`−${formatPrice(order.discountAmount)}`} />}
              <Row label={`Vergi (%${order.taxRate})`} value={formatPrice(order.taxAmount)} />
              <Row label="Kargo" value={formatPrice(order.shippingFee)} />
              <Row label="Toplam" value={formatPrice(order.grandTotal)} strong />
            </dl>
            {order.coupons.length > 0 && (
              <div className="mt-4 border-t border-border pt-4">
                <p className="text-xs uppercase tracking-widest text-muted-foreground">Kuponlar</p>
                <ul className="mt-2 space-y-1 text-sm">
                  {order.coupons.map((c) => (
                    <li key={c.code} className="flex justify-between">
                      <span>{c.code}</span>
                      <span className="font-mono">−{formatPrice(c.discountAmount)}</span>
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </AdminSection>

          <AdminSection title="Referanslar">
            <dl className="space-y-2 text-xs">
              <Row label="Müşteri" value={order.userId ?? order.guestCustomerId ?? "—"} mono />
              <Row label="Kargo Firması" value={order.shippingCompanyId} mono />
              <Row label="Gönderi" value={order.shipmentId} mono />
            </dl>
          </AdminSection>
        </div>
      </div>
    </div>
  )
}

function Row({ label, value, strong, mono }: { label: string; value: string; strong?: boolean; mono?: boolean }) {
  return (
    <div className={`flex justify-between gap-3 ${strong ? "border-t border-border pt-2 font-medium" : ""}`}>
      <dt className="text-muted-foreground">{label}</dt>
      <dd className={mono ? "truncate font-mono" : "font-mono"}>{value}</dd>
    </div>
  )
}
