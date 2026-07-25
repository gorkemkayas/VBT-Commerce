"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import * as paymentsApi from "@/lib/api/payments"
import type { Payment } from "@/lib/api/types"
import { formatDateTime, formatPrice } from "@/lib/format"
import { paymentStatusLabels, paymentStatusTone } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Badge } from "@/components/ui/badge"
import { EmptyState } from "@/components/ui/empty-state"
import { PageSpinner } from "@/components/ui/spinner"

export function PaymentDetail({ paymentId }: { paymentId: string }) {
  const [payment, setPayment] = useState<Payment | null>(null)
  const [loading, setLoading] = useState(true)
  const [notFound, setNotFound] = useState(false)

  useEffect(() => {
    paymentsApi
      .getPaymentById(paymentId)
      .then(setPayment)
      .catch(() => setNotFound(true))
      .finally(() => setLoading(false))
  }, [paymentId])

  if (loading) return <PageSpinner label="Ödeme yükleniyor…" />
  if (notFound || !payment) return <EmptyState title="Ödeme bulunamadı" />

  return (
    <div>
      <Link href="/admin/payments" className="mb-4 flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
        <ChevronLeft className="h-3.5 w-3.5" strokeWidth={1.5} />
        Ödemelere Dön
      </Link>
      <AdminPageHeader
        title={formatPrice(payment.amount)}
        description={formatDateTime(payment.createdAt)}
        action={<Badge tone={paymentStatusTone[payment.status]}>{paymentStatusLabels[payment.status]}</Badge>}
      />

      <AdminSection title="Detaylar">
        <dl className="space-y-3 text-sm">
          <Row label="Sağlayıcı İşlem No" value={payment.providerPaymentId} mono />
          <Row label="Sipariş" value={payment.orderId} mono />
          <Row label="Kart" value={`${payment.cardAssociation ?? "—"} ${payment.cardFamily ?? ""} •••• ${payment.cardLastFourDigits ?? "----"}`} />
          {payment.refundedAt && <Row label="İade Tarihi" value={formatDateTime(payment.refundedAt)} />}
        </dl>
      </AdminSection>
    </div>
  )
}

function Row({ label, value, mono }: { label: string; value: string; mono?: boolean }) {
  return (
    <div className="flex justify-between gap-3 border-b border-border pb-3">
      <dt className="text-muted-foreground">{label}</dt>
      <dd className={mono ? "font-mono text-xs" : ""}>{value}</dd>
    </div>
  )
}
