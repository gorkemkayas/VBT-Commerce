"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import * as customersApi from "@/lib/api/customers"
import type { CustomerListItem } from "@/lib/api/types"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20

export function CustomerList() {
  const [customers, setCustomers] = useState<CustomerListItem[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    customersApi
      .getCustomers(pageNumber, PAGE_SIZE)
      .then((result) => {
        if (cancelled) return
        setCustomers(result.items)
        setTotalPages(result.totalPages || 1)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [pageNumber])

  return (
    <div>
      <AdminPageHeader title="Müşteriler" description="Kayıtlı üye listesi." />
      <AdminSection>
        {loading ? (
          <PageSpinner label="Müşteriler yükleniyor…" />
        ) : customers.length === 0 ? (
          <EmptyState title="Müşteri bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH>Kullanıcı Id</TH>
                  <TH>Telefon</TH>
                  <TH>Adres Sayısı</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {customers.map((c) => (
                  <TR key={c.id}>
                    <TD className="font-mono text-xs">{c.userId}</TD>
                    <TD className="text-muted-foreground">{c.phoneNumber ?? "—"}</TD>
                    <TD className="tabular-nums">{c.addressCount}</TD>
                    <TD className="text-right">
                      <Link href={`/admin/customers/${c.userId}`} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
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
