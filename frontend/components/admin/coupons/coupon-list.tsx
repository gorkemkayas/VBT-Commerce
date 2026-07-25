"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { Plus } from "lucide-react"
import * as pricingApi from "@/lib/api/pricing"
import type { Coupon } from "@/lib/api/types"
import { formatDate, formatPrice } from "@/lib/format"
import { couponDiscountTypeLabels } from "@/lib/enum-labels"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Badge } from "@/components/ui/badge"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20

export function CouponList() {
  const [coupons, setCoupons] = useState<Coupon[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  async function load() {
    setLoading(true)
    try {
      const result = await pricingApi.getCoupons(pageNumber, PAGE_SIZE)
      setCoupons(result.items)
      setTotalPages(result.totalPages || 1)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber])

  async function handleDeactivate(id: string) {
    if (!window.confirm("Bu kuponu pasifleştirmek istediğinize emin misiniz?")) return
    await pricingApi.deactivateCoupon(id)
    await load()
  }

  return (
    <div>
      <AdminPageHeader
        title="Kuponlar"
        description="İndirim kodları."
        action={
          <Link href="/admin/coupons/new" className="flex items-center gap-1.5 bg-foreground px-4 py-2 text-xs font-medium uppercase tracking-widest text-background hover:opacity-80">
            <Plus className="h-3.5 w-3.5" strokeWidth={2} />
            Yeni Kupon
          </Link>
        }
      />
      <AdminSection>
        {loading ? (
          <PageSpinner label="Kuponlar yükleniyor…" />
        ) : coupons.length === 0 ? (
          <EmptyState title="Kupon bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH>Kod</TH>
                  <TH>İndirim</TH>
                  <TH>Kapsam</TH>
                  <TH>Geçerlilik</TH>
                  <TH>Durum</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {coupons.map((c) => (
                  <TR key={c.id}>
                    <TD className="font-mono font-medium">{c.code}</TD>
                    <TD className="text-muted-foreground">
                      {c.discountType === "Percentage" ? `%${c.discountValue}` : formatPrice(c.discountValue)}
                    </TD>
                    <TD className="text-muted-foreground">{couponDiscountTypeLabels[c.discountType]}</TD>
                    <TD className="text-muted-foreground">
                      {formatDate(c.startDate)} – {formatDate(c.endDate)}
                    </TD>
                    <TD>
                      <Badge tone={c.isActive ? "success" : "neutral"}>{c.isActive ? "Aktif" : "Pasif"}</Badge>
                    </TD>
                    <TD className="text-right">
                      <div className="flex justify-end gap-4 text-xs font-medium uppercase tracking-widest">
                        <Link href={`/admin/coupons/${c.code}`} className="text-muted-foreground hover:text-foreground">
                          Düzenle
                        </Link>
                        {c.isActive && (
                          <button type="button" onClick={() => handleDeactivate(c.id)} className="text-muted-foreground hover:text-foreground">
                            Pasifleştir
                          </button>
                        )}
                      </div>
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
