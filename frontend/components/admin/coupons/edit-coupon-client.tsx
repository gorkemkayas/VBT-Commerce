"use client"

import { useEffect, useState } from "react"
import { getCouponByCode } from "@/lib/api/pricing"
import type { Coupon } from "@/lib/api/types"
import { CouponForm } from "@/components/admin/coupons/coupon-form"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

export function EditCouponClient({ code }: { code: string }) {
  const [coupon, setCoupon] = useState<Coupon | null>(null)
  const [loading, setLoading] = useState(true)
  const [notFound, setNotFound] = useState(false)

  useEffect(() => {
    getCouponByCode(code)
      .then(setCoupon)
      .catch(() => setNotFound(true))
      .finally(() => setLoading(false))
  }, [code])

  if (loading) return <PageSpinner label="Kupon yükleniyor…" />
  if (notFound || !coupon) return <EmptyState title="Kupon bulunamadı" />

  return <CouponForm coupon={coupon} />
}
