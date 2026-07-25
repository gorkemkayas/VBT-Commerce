"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import * as shippingApi from "@/lib/api/shipping"
import type { Shipment, ShipmentStatus } from "@/lib/api/types"
import { formatDateTime } from "@/lib/format"
import { shipmentStatusLabels, shipmentStatusTone } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Select } from "@/components/ui/select"
import { Badge } from "@/components/ui/badge"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20
const statuses: ShipmentStatus[] = ["Pending", "Shipped", "InTransit", "Delivered", "Cancelled"]

export function ShipmentList() {
  const [shipments, setShipments] = useState<Shipment[]>([])
  const [status, setStatus] = useState<ShipmentStatus | "">("")
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    shippingApi
      .getShipments({ status: status || undefined, pageNumber, pageSize: PAGE_SIZE })
      .then((result) => {
        if (cancelled) return
        setShipments(result.items)
        setTotalPages(result.totalPages || 1)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [status, pageNumber])

  return (
    <div>
      <AdminPageHeader title="Gönderiler" description="Sipariş kargo durumları." />
      <AdminSection>
        <div className="mb-4">
          <Select
            value={status}
            onChange={(e) => {
              setPageNumber(1)
              setStatus(e.target.value as ShipmentStatus | "")
            }}
            className="max-w-[200px]"
          >
            <option value="">Tüm durumlar</option>
            {statuses.map((s) => (
              <option key={s} value={s}>
                {shipmentStatusLabels[s]}
              </option>
            ))}
          </Select>
        </div>

        {loading ? (
          <PageSpinner label="Gönderiler yükleniyor…" />
        ) : shipments.length === 0 ? (
          <EmptyState title="Gönderi bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH>Sipariş</TH>
                  <TH>Takip No</TH>
                  <TH>Güncellenme</TH>
                  <TH>Durum</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {shipments.map((s) => (
                  <TR key={s.id}>
                    <TD className="font-mono text-xs">{s.orderId.slice(0, 8).toUpperCase()}</TD>
                    <TD className="text-muted-foreground">{s.trackingNumber ?? "—"}</TD>
                    <TD className="text-muted-foreground">{s.updatedAt ? formatDateTime(s.updatedAt) : formatDateTime(s.createdAt)}</TD>
                    <TD>
                      <Badge tone={shipmentStatusTone[s.status]}>{shipmentStatusLabels[s.status]}</Badge>
                    </TD>
                    <TD className="text-right">
                      <Link href={`/admin/shipments/${s.id}`} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
                        Detay
                      </Link>
                    </TD>
                  </TR>
                ))}
              </TBody>
            </Table>
            <div className="mt-4">
              <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
            </div>
          </>
        )}
      </AdminSection>
    </div>
  )
}
