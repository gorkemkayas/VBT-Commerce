"use client"

import { Fragment, useEffect, useMemo, useState } from "react"
import Image from "next/image"
import { ChevronRight } from "lucide-react"
import * as inventoryApi from "@/lib/api/inventory"
import type { StockReservation } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { formatDateTime } from "@/lib/format"
import { buildCatalogIndex, variantLabel, type CatalogEntry } from "@/lib/catalog-index"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Badge } from "@/components/ui/badge"
import { Select } from "@/components/ui/select"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { FormMessage } from "@/components/ui/form-message"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20

type StatusFilter = "" | "active" | "confirmed" | "released"

function reservationStatus(r: StockReservation): { label: string; tone: "neutral" | "success" | "warning" | "info" } {
  if (r.isReleased) return { label: "Serbest Bırakıldı", tone: "neutral" }
  if (r.isConfirmed) return { label: "Onaylandı", tone: "success" }
  if (new Date(r.expiresAt).getTime() < Date.now()) return { label: "Süresi Doldu", tone: "warning" }
  return { label: "Aktif", tone: "info" }
}

function ReservationCells({ r }: { r: StockReservation }) {
  const status = reservationStatus(r)
  return (
    <>
      <TD className="font-mono text-xs" title={r.referenceId}>
        {r.referenceId.slice(0, 8).toUpperCase()}
      </TD>
      <TD className="tabular-nums">{r.quantity}</TD>
      <TD>
        <Badge tone={status.tone}>{status.label}</Badge>
      </TD>
      <TD className="text-xs text-muted-foreground">{formatDateTime(r.createdAt)}</TD>
    </>
  )
}

function ReservationActions({
  r,
  onAction,
}: {
  r: StockReservation
  onAction: (referenceId: string, action: "confirm" | "release") => void
}) {
  if (r.isReleased || r.isConfirmed) return null
  return (
    <div className="flex justify-end gap-3 text-xs font-medium uppercase tracking-widest">
      <button type="button" onClick={() => onAction(r.referenceId, "confirm")} className="text-muted-foreground hover:text-foreground">
        Onayla
      </button>
      <button type="button" onClick={() => onAction(r.referenceId, "release")} className="text-muted-foreground hover:text-foreground">
        Serbest Bırak
      </button>
    </div>
  )
}

export function ReservationList() {
  const [reservations, setReservations] = useState<StockReservation[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("")
  const [loading, setLoading] = useState(true)
  const [catalogIndex, setCatalogIndex] = useState<Map<string, CatalogEntry> | null>(null)
  const [expandedGroups, setExpandedGroups] = useState<Set<string>>(new Set())

  const [referenceId, setReferenceId] = useState("")
  const [actionMessage, setActionMessage] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const result = await inventoryApi.getStockReservations({
        pageNumber,
        pageSize: PAGE_SIZE,
        isConfirmed: statusFilter === "confirmed" ? true : statusFilter === "active" ? false : undefined,
        isReleased: statusFilter === "released" ? true : statusFilter === "active" || statusFilter === "confirmed" ? false : undefined,
      })
      setReservations(result.items)
      setTotalPages(result.totalPages || 1)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber, statusFilter])

  useEffect(() => {
    buildCatalogIndex()
      .then(setCatalogIndex)
      .catch(() => setCatalogIndex(new Map()))
  }, [])

  const groups = useMemo(() => {
    const map = new Map<string, { key: string; productName: string; productImage: string | null; rows: StockReservation[] }>()
    for (const r of reservations) {
      const entry = catalogIndex?.get(r.sellableItemId) ?? null
      const key = entry?.productId ?? r.sellableItemId
      if (!map.has(key)) {
        map.set(key, {
          key,
          productName: entry?.productName ?? "Bilinmeyen ürün",
          productImage: entry?.productImage ?? null,
          rows: [],
        })
      }
      map.get(key)!.rows.push(r)
    }
    return Array.from(map.values())
  }, [reservations, catalogIndex])

  function toggleGroup(key: string) {
    setExpandedGroups((prev) => {
      const next = new Set(prev)
      if (next.has(key)) next.delete(key)
      else next.add(key)
      return next
    })
  }

  async function handleReferenceAction(refId: string, action: "confirm" | "release") {
    try {
      if (action === "confirm") await inventoryApi.confirmReservationsByReference(refId)
      else await inventoryApi.releaseReservationsByReference(refId)
      await load()
    } catch (err) {
      window.alert(err instanceof ApiError ? err.message : "İşlem başarısız.")
    }
  }

  async function handleManualAction(action: "confirm" | "release") {
    setActionError(null)
    setActionMessage(null)
    try {
      if (action === "confirm") await inventoryApi.confirmReservationsByReference(referenceId)
      else await inventoryApi.releaseReservationsByReference(referenceId)
      setActionMessage("Rezervasyon(lar) güncellendi.")
      await load()
    } catch (err) {
      setActionError(err instanceof ApiError ? err.message : "İşlem başarısız.")
    }
  }

  return (
    <div>
      <AdminPageHeader
        title="Stok Rezervasyonları"
        description="Ürünlerinizin ve varyantlarının rezervasyon geçmişi. Normal akışta rezervasyon/onay/serbest bırakma checkout tarafından otomatik yapılır."
      />

      <AdminSection title="Referansa Göre İşlem">
        <div className="flex flex-wrap items-end gap-3">
          <div className="min-w-[280px] flex-1">
            <Label>Reference Id (sipariş/işlem referansı)</Label>
            <Input value={referenceId} onChange={(e) => setReferenceId(e.target.value)} />
          </div>
          <button
            type="button"
            onClick={() => handleManualAction("confirm")}
            disabled={!referenceId}
            className="border border-foreground px-6 py-2.5 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50"
          >
            Onayla
          </button>
          <button
            type="button"
            onClick={() => handleManualAction("release")}
            disabled={!referenceId}
            className="border border-border px-6 py-2.5 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:border-foreground hover:text-foreground disabled:opacity-50"
          >
            Serbest Bırak
          </button>
        </div>
        {actionMessage && (
          <div className="mt-3">
            <FormMessage tone="success">{actionMessage}</FormMessage>
          </div>
        )}
        {actionError && (
          <div className="mt-3">
            <FormMessage tone="error">{actionError}</FormMessage>
          </div>
        )}
      </AdminSection>

      <div className="mt-6">
        <AdminSection>
          <div className="mb-4">
            <Select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value as StatusFilter)} className="max-w-[200px]">
              <option value="">Tüm durumlar</option>
              <option value="active">Aktif</option>
              <option value="confirmed">Onaylanmış</option>
              <option value="released">Serbest Bırakılmış</option>
            </Select>
          </div>
          {loading ? (
            <PageSpinner label="Rezervasyonlar yükleniyor…" />
          ) : reservations.length === 0 ? (
            <EmptyState title="Rezervasyon bulunamadı" />
          ) : (
            <>
              <Table>
                <THead>
                  <TR>
                    <TH>Ürün / Varyant</TH>
                    <TH>Referans</TH>
                    <TH>Miktar</TH>
                    <TH>Durum</TH>
                    <TH>Oluşturulma</TH>
                    <TH></TH>
                  </TR>
                </THead>
                <TBody>
                  {groups.map((group) => {
                    if (group.rows.length === 1) {
                      const r = group.rows[0]
                      return (
                        <TR key={group.key}>
                          <TD>
                            <div className="flex items-center gap-3">
                              <div className="relative h-9 w-9 shrink-0 overflow-hidden bg-secondary">
                                <Image src={group.productImage || "/placeholder.svg"} alt="" fill sizes="36px" className="object-cover" />
                              </div>
                              <p className="truncate text-sm">{group.productName}</p>
                            </div>
                          </TD>
                          <ReservationCells r={r} />
                          <TD className="text-right">
                            <ReservationActions r={r} onAction={handleReferenceAction} />
                          </TD>
                        </TR>
                      )
                    }

                    const isOpen = expandedGroups.has(group.key)

                    return (
                      <Fragment key={group.key}>
                        <TR className="cursor-pointer bg-secondary/30 hover:bg-secondary/50" onClick={() => toggleGroup(group.key)}>
                          <TD colSpan={6}>
                            <div className="flex items-center gap-3">
                              <ChevronRight
                                className={`h-3.5 w-3.5 shrink-0 text-muted-foreground transition-transform ${isOpen ? "rotate-90" : ""}`}
                                strokeWidth={1.5}
                              />
                              <div className="relative h-9 w-9 shrink-0 overflow-hidden bg-secondary">
                                <Image src={group.productImage || "/placeholder.svg"} alt="" fill sizes="36px" className="object-cover" />
                              </div>
                              <p className="text-sm font-medium">{group.productName}</p>
                              <Badge tone="neutral">{group.rows.length} kayıt</Badge>
                            </div>
                          </TD>
                        </TR>
                        {isOpen &&
                          group.rows.map((r) => {
                            const entry = catalogIndex?.get(r.sellableItemId) ?? null
                            return (
                              <TR key={r.id}>
                                <TD>
                                  <div className="flex items-center gap-3 pl-8">
                                    <div className="relative h-8 w-8 shrink-0 overflow-hidden bg-secondary">
                                      <Image
                                        src={entry?.variantImage || group.productImage || "/placeholder.svg"}
                                        alt=""
                                        fill
                                        sizes="32px"
                                        className="object-cover"
                                      />
                                    </div>
                                    <p className="truncate text-sm text-muted-foreground">
                                      {entry?.variant ? variantLabel(entry.variant) : "—"}
                                    </p>
                                  </div>
                                </TD>
                                <ReservationCells r={r} />
                                <TD className="text-right">
                                  <ReservationActions r={r} onAction={handleReferenceAction} />
                                </TD>
                              </TR>
                            )
                          })}
                      </Fragment>
                    )
                  })}
                </TBody>
              </Table>
              <div className="mt-4">
                <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
              </div>
            </>
          )}
        </AdminSection>
      </div>
    </div>
  )
}
