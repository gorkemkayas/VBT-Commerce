"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import * as paymentsApi from "@/lib/api/payments"
import type { Payment, PaymentStatus } from "@/lib/api/types"
import { formatDateTime, formatPrice } from "@/lib/format"
import { paymentStatusLabels, paymentStatusTone } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Select } from "@/components/ui/select"
import { Badge } from "@/components/ui/badge"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20
const statuses: PaymentStatus[] = ["Succeeded", "Refunded"]

export function PaymentList() {
  const [payments, setPayments] = useState<Payment[]>([])
  const [status, setStatus] = useState<PaymentStatus | "">("")
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    paymentsApi
      .getPayments({ status: status || undefined, pageNumber, pageSize: PAGE_SIZE })
      .then((result) => {
        if (cancelled) return
        setPayments(result.items)
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
      <AdminPageHeader title="Ödemeler" description="Tüm ödeme kayıtları." />
      <AdminSection>
        <div className="mb-4">
          <Select
            value={status}
            onChange={(e) => {
              setPageNumber(1)
              setStatus(e.target.value as PaymentStatus | "")
            }}
            className="max-w-[200px]"
          >
            <option value="">Tüm durumlar</option>
            {statuses.map((s) => (
              <option key={s} value={s}>
                {paymentStatusLabels[s]}
              </option>
            ))}
          </Select>
        </div>

        {loading ? (
          <PageSpinner label="Ödemeler yükleniyor…" />
        ) : payments.length === 0 ? (
          <EmptyState title="Ödeme bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH>Tarih</TH>
                  <TH>Kart</TH>
                  <TH>Tutar</TH>
                  <TH>Durum</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {payments.map((p) => (
                  <TR key={p.id}>
                    <TD className="text-muted-foreground">{formatDateTime(p.createdAt)}</TD>
                    <TD className="text-muted-foreground">
                      {p.cardAssociation ?? "—"} •••• {p.cardLastFourDigits ?? "----"}
                    </TD>
                    <TD className="tabular-nums">{formatPrice(p.amount)}</TD>
                    <TD>
                      <Badge tone={paymentStatusTone[p.status]}>{paymentStatusLabels[p.status]}</Badge>
                    </TD>
                    <TD className="text-right">
                      <Link href={`/admin/payments/${p.id}`} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
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
