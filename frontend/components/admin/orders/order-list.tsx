"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import * as ordersApi from "@/lib/api/orders"
import type { Order, OrderStatus } from "@/lib/api/types"
import { formatDate, formatPrice } from "@/lib/format"
import { orderStatusLabels, orderStatusTone } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Select } from "@/components/ui/select"
import { Badge } from "@/components/ui/badge"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20
const statuses: OrderStatus[] = ["Pending", "Confirmed", "Cancelled"]

export function OrderList() {
  const [orders, setOrders] = useState<Order[]>([])
  const [status, setStatus] = useState<OrderStatus | "">("")
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    ordersApi
      .getAdminOrders({ status: status || undefined, pageNumber, pageSize: PAGE_SIZE })
      .then((result) => {
        if (cancelled) return
        setOrders(result.items)
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
      <AdminPageHeader title="Siparişler" description="Tüm müşteri ve misafir siparişleri." />
      <AdminSection>
        <div className="mb-4">
          <Select
            value={status}
            onChange={(e) => {
              setPageNumber(1)
              setStatus(e.target.value as OrderStatus | "")
            }}
            className="max-w-[200px]"
          >
            <option value="">Tüm durumlar</option>
            {statuses.map((s) => (
              <option key={s} value={s}>
                {orderStatusLabels[s]}
              </option>
            ))}
          </Select>
        </div>

        {loading ? (
          <PageSpinner label="Siparişler yükleniyor…" />
        ) : orders.length === 0 ? (
          <EmptyState title="Sipariş bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH>Sipariş</TH>
                  <TH>Tarih</TH>
                  <TH>Müşteri Tipi</TH>
                  <TH>Durum</TH>
                  <TH>Toplam</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {orders.map((order) => (
                  <TR key={order.id}>
                    <TD className="font-mono text-xs">{order.id.slice(0, 8).toUpperCase()}</TD>
                    <TD className="text-muted-foreground">{formatDate(order.createdAt)}</TD>
                    <TD className="text-muted-foreground">{order.userId ? "Üye" : "Misafir"}</TD>
                    <TD>
                      <Badge tone={orderStatusTone[order.status]}>{orderStatusLabels[order.status]}</Badge>
                    </TD>
                    <TD className="tabular-nums">{formatPrice(order.grandTotal)}</TD>
                    <TD className="text-right">
                      <Link href={`/admin/orders/${order.id}`} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
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
