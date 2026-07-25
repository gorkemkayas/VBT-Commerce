"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import * as shippingApi from "@/lib/api/shipping"
import type { Shipment, ShipmentStatus } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { formatDateTime } from "@/lib/format"
import { shipmentStatusLabels, shipmentStatusTone } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { ShipmentTimeline } from "@/components/shipment-timeline"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { FormMessage } from "@/components/ui/form-message"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const statuses: ShipmentStatus[] = ["Pending", "Shipped", "InTransit", "Delivered", "Cancelled"]

export function ShipmentDetail({ shipmentId }: { shipmentId: string }) {
  const [shipment, setShipment] = useState<Shipment | null>(null)
  const [loading, setLoading] = useState(true)
  const [notFound, setNotFound] = useState(false)
  const [status, setStatus] = useState<ShipmentStatus>("Pending")
  const [trackingNumber, setTrackingNumber] = useState("")
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  async function load() {
    try {
      const result = await shippingApi.getShipmentById(shipmentId)
      setShipment(result)
      setStatus(result.status)
      setTrackingNumber(result.trackingNumber ?? "")
    } catch {
      setNotFound(true)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [shipmentId])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      await shippingApi.updateShipmentStatus(shipmentId, status, trackingNumber || null)
      setSuccess(true)
      await load()
      setTimeout(() => setSuccess(false), 2000)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Güncellenemedi.")
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <PageSpinner label="Gönderi yükleniyor…" />
  if (notFound || !shipment) return <EmptyState title="Gönderi bulunamadı" />

  return (
    <div>
      <Link href="/admin/shipments" className="mb-4 flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
        <ChevronLeft className="h-3.5 w-3.5" strokeWidth={1.5} />
        Gönderilere Dön
      </Link>
      <AdminPageHeader
        title={`Gönderi — Sipariş ${shipment.orderId.slice(0, 8).toUpperCase()}`}
        action={<Badge tone={shipmentStatusTone[shipment.status]}>{shipmentStatusLabels[shipment.status]}</Badge>}
      />

      <AdminSection title="Durumu Güncelle">
        <form onSubmit={handleSubmit} className="max-w-md space-y-4">
          <div>
            <Label>Durum</Label>
            <Select value={status} onChange={(e) => setStatus(e.target.value as ShipmentStatus)}>
              {statuses.map((s) => (
                <option key={s} value={s}>
                  {shipmentStatusLabels[s]}
                </option>
              ))}
            </Select>
          </div>
          <div>
            <Label>Takip Numarası</Label>
            <Input value={trackingNumber} onChange={(e) => setTrackingNumber(e.target.value)} />
          </div>
          {error && <FormMessage tone="error">{error}</FormMessage>}
          {success && <FormMessage tone="success">Güncellendi.</FormMessage>}
          <button type="submit" disabled={saving} className="border border-foreground px-6 py-2.5 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
            {saving ? "Kaydediliyor…" : "Kaydet"}
          </button>
        </form>
      </AdminSection>

      <div className="mt-6 grid grid-cols-1 gap-6 lg:grid-cols-2">
        <AdminSection title="Detaylar">
          <dl className="space-y-3 text-sm">
            <div className="flex justify-between gap-3 border-b border-border pb-3">
              <dt className="text-muted-foreground">Oluşturulma</dt>
              <dd>{formatDateTime(shipment.createdAt)}</dd>
            </div>
            {shipment.updatedAt && (
              <div className="flex justify-between gap-3 border-b border-border pb-3">
                <dt className="text-muted-foreground">Son Güncelleme</dt>
                <dd>{formatDateTime(shipment.updatedAt)}</dd>
              </div>
            )}
            <div className="flex justify-between gap-3">
              <dt className="text-muted-foreground">Kargo Firması Id</dt>
              <dd className="font-mono text-xs">{shipment.shippingCompanyId}</dd>
            </div>
          </dl>
        </AdminSection>

        <AdminSection title="Durum Geçmişi">
          <ShipmentTimeline history={shipment.history} />
        </AdminSection>
      </div>
    </div>
  )
}
