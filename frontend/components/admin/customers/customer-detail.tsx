"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import * as customersApi from "@/lib/api/customers"
import type { Customer } from "@/lib/api/types"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

export function CustomerDetail({ userId }: { userId: string }) {
  const [customer, setCustomer] = useState<Customer | null>(null)
  const [loading, setLoading] = useState(true)
  const [notFound, setNotFound] = useState(false)

  useEffect(() => {
    customersApi
      .getCustomerByUserId(userId)
      .then(setCustomer)
      .catch(() => setNotFound(true))
      .finally(() => setLoading(false))
  }, [userId])

  if (loading) return <PageSpinner label="Müşteri yükleniyor…" />
  if (notFound || !customer) return <EmptyState title="Müşteri bulunamadı" />

  return (
    <div>
      <Link href="/admin/customers" className="mb-4 flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
        <ChevronLeft className="h-3.5 w-3.5" strokeWidth={1.5} />
        Müşterilere Dön
      </Link>
      <AdminPageHeader title="Müşteri Detayı" description={customer.userId} />

      <AdminSection title="Profil">
        <dl className="space-y-3 text-sm">
          <div className="flex justify-between gap-3 border-b border-border pb-3">
            <dt className="text-muted-foreground">Telefon</dt>
            <dd>{customer.phoneNumber ?? "—"}</dd>
          </div>
          <div className="flex justify-between gap-3">
            <dt className="text-muted-foreground">Doğum Tarihi</dt>
            <dd>{customer.dateOfBirth ?? "—"}</dd>
          </div>
        </dl>
      </AdminSection>

      <div className="mt-6">
        <AdminSection title={`Adresler (${customer.addresses.length})`}>
          {customer.addresses.length === 0 ? (
            <p className="text-sm text-muted-foreground">Kayıtlı adres yok.</p>
          ) : (
            <div className="grid gap-4 sm:grid-cols-2">
              {customer.addresses.map((address) => (
                <article key={address.id} className="border border-border p-4">
                  <div className="mb-2 flex items-center justify-between">
                    <span className="text-sm font-semibold">{address.label}</span>
                    {address.isDefault && <span className="text-[10px] font-medium uppercase tracking-wider text-muted-foreground">Varsayılan</span>}
                  </div>
                  <p className="text-sm">{address.recipientName}</p>
                  <p className="text-sm text-muted-foreground">
                    {address.addressLine1}
                    {address.addressLine2 ? `, ${address.addressLine2}` : ""}
                  </p>
                  <p className="text-sm text-muted-foreground">
                    {address.district}/{address.city}, {address.postalCode}
                  </p>
                </article>
              ))}
            </div>
          )}
        </AdminSection>
      </div>
    </div>
  )
}
