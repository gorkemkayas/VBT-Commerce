"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { useSearchParams } from "next/navigation"
import { Check, Copy } from "lucide-react"
import { useAuth } from "@/lib/auth-context"
import { getGuestOrder, getMyOrder, getMyOrderShipment } from "@/lib/api/orders"
import type { Order, ShipmentTracking } from "@/lib/api/types"
import { formatDateTime, formatPrice } from "@/lib/format"
import { orderStatusLabels, shipmentStatusLabels, shipmentStatusTone } from "@/lib/enum-labels"
import { PageSpinner } from "@/components/ui/spinner"
import { FormMessage } from "@/components/ui/form-message"
import { Badge } from "@/components/ui/badge"
import { ShipmentTimeline } from "@/components/shipment-timeline"

export function OrderConfirmation({ orderId }: { orderId: string }) {
  const searchParams = useSearchParams()
  const guestCustomerId = searchParams.get("guestCustomerId")
  const { isAuthenticated, bootstrapping } = useAuth()

  const [order, setOrder] = useState<Order | null>(null)
  const [shipment, setShipment] = useState<ShipmentTracking | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [copied, setCopied] = useState(false)

  useEffect(() => {
    if (bootstrapping) return
    let cancelled = false
    ;(async () => {
      try {
        const result = guestCustomerId ? await getGuestOrder(guestCustomerId, orderId) : await getMyOrder(orderId)
        if (!cancelled) setOrder(result)
      } catch {
        if (!cancelled) setError("Sipariş bulunamadı.")
        return
      }
      if (!guestCustomerId) {
        try {
          const result = await getMyOrderShipment(orderId)
          if (!cancelled) setShipment(result)
        } catch {
          // Kargo henüz oluşturulmamış olabilir — bu durumda bölüm gösterilmez.
        }
      }
    })()
    return () => {
      cancelled = true
    }
  }, [orderId, guestCustomerId, isAuthenticated, bootstrapping])

  function copyReference() {
    navigator.clipboard.writeText(`Sipariş No: ${orderId}${guestCustomerId ? `\nMisafir ID: ${guestCustomerId}` : ""}`)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  if (bootstrapping || (!order && !error)) return <PageSpinner label="Sipariş bilgisi yükleniyor…" />

  if (error || !order) {
    return (
      <div className="mx-auto max-w-2xl px-4 py-24 text-center">
        <FormMessage tone="error">{error ?? "Sipariş bulunamadı."}</FormMessage>
        <Link href="/shop" className="mt-8 inline-block border border-foreground px-8 py-4 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background">
          Alışverişe Devam Et
        </Link>
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-2xl px-4 py-16 md:px-6">
      <div className="flex flex-col items-center text-center">
        <div className="flex h-14 w-14 items-center justify-center border border-foreground">
          <Check className="h-6 w-6" strokeWidth={1.5} />
        </div>
        <h1 className="mt-6 font-serif text-3xl font-medium tracking-tight">Siparişiniz Alındı</h1>
        <p className="mt-3 max-w-md text-sm text-muted-foreground">
          Teşekkürler. Siparişiniz <strong className="text-foreground">{orderStatusLabels[order.status]}</strong> durumunda.
        </p>

        {guestCustomerId && (
          <div className="mt-6 w-full border border-border p-4 text-left">
            <p className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Takip Referansınız</p>
            <p className="mt-1 text-xs text-muted-foreground">Bu bilgileri saklayın — ikisi de sipariş sorgulama için gereklidir.</p>
            <div className="mt-3 space-y-2 border border-border bg-secondary px-3 py-2">
              <div className="flex items-center justify-between gap-2 font-mono text-xs">
                <span className="truncate">
                  <span className="text-muted-foreground">Sipariş No: </span>
                  {orderId}
                </span>
              </div>
              <div className="flex items-center justify-between gap-2 font-mono text-xs">
                <span className="truncate">
                  <span className="text-muted-foreground">Misafir ID: </span>
                  {guestCustomerId}
                </span>
              </div>
              <div className="flex justify-end pt-1">
                <button
                  type="button"
                  onClick={copyReference}
                  className="flex items-center gap-1.5 text-[10px] font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground"
                >
                  {copied ? <Check className="h-3.5 w-3.5" strokeWidth={1.5} /> : <Copy className="h-3.5 w-3.5" strokeWidth={1.5} />}
                  {copied ? "Kopyalandı" : "İkisini de kopyala"}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>

      <div className="mt-10 border border-border">
        <div className="flex items-center justify-between border-b border-border px-5 py-4">
          <span className="text-sm font-medium">Sipariş No</span>
          <span className="font-mono text-xs text-muted-foreground">{order.id}</span>
        </div>
        <ul className="divide-y divide-border">
          {order.items.map((item, i) => (
            <li key={i} className="flex items-center justify-between px-5 py-3 text-sm">
              <span className="text-muted-foreground">
                {item.quantity} × {formatPrice(item.unitPrice)}
              </span>
              <span className="font-mono">{formatPrice(item.lineSubtotal)}</span>
            </li>
          ))}
        </ul>
        <div className="space-y-2 border-t border-border px-5 py-4 text-sm">
          <div className="flex justify-between">
            <span className="text-muted-foreground">Ara Toplam</span>
            <span className="font-mono">{formatPrice(order.subtotal)}</span>
          </div>
          {order.discountAmount > 0 && (
            <div className="flex justify-between">
              <span className="text-muted-foreground">İndirim</span>
              <span className="font-mono">−{formatPrice(order.discountAmount)}</span>
            </div>
          )}
          <div className="flex justify-between">
            <span className="text-muted-foreground">Vergi</span>
            <span className="font-mono">{formatPrice(order.taxAmount)}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-muted-foreground">Kargo</span>
            <span className="font-mono">{formatPrice(order.shippingFee)}</span>
          </div>
          <div className="flex justify-between border-t border-border pt-2 text-base font-medium">
            <span>Toplam</span>
            <span className="font-mono">{formatPrice(order.grandTotal)}</span>
          </div>
        </div>
        <div className="border-t border-border px-5 py-4 text-sm text-muted-foreground">
          <p className="text-foreground">{order.recipientName}</p>
          <p>
            {order.addressLine1}
            {order.addressLine2 ? `, ${order.addressLine2}` : ""}
          </p>
          <p>
            {order.district}/{order.city}, {order.postalCode}
          </p>
          <p className="mt-2 text-xs">{formatDateTime(order.createdAt)}</p>
        </div>
      </div>

      {shipment && (
        <div className="mt-8 border border-border p-5">
          <div className="flex items-center justify-between">
            <h2 className="text-sm font-medium">Kargo Takibi</h2>
            <Badge tone={shipmentStatusTone[shipment.status]}>{shipmentStatusLabels[shipment.status]}</Badge>
          </div>
          {shipment.trackingNumber && (
            <p className="mt-2 text-xs text-muted-foreground">Takip No: {shipment.trackingNumber}</p>
          )}
          <div className="mt-5">
            <ShipmentTimeline history={shipment.history} />
          </div>
        </div>
      )}

      <Link
        href="/shop"
        className="mt-8 block w-full border border-foreground py-4 text-center text-xs font-medium uppercase tracking-widest transition-colors hover:bg-foreground hover:text-background"
      >
        Alışverişe Devam Et
      </Link>
    </div>
  )
}
