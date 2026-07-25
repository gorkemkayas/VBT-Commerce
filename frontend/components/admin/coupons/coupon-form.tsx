"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import * as pricingApi from "@/lib/api/pricing"
import type { Coupon, CouponDiscountType, CouponScopeType } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { FormMessage } from "@/components/ui/form-message"

function toLocalInput(iso: string) {
  const d = new Date(iso)
  const pad = (n: number) => String(n).padStart(2, "0")
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}

export function CouponForm({ coupon }: { coupon?: Coupon }) {
  const router = useRouter()
  const isEdit = !!coupon

  const [code, setCode] = useState(coupon?.code ?? "")
  const [discountType, setDiscountType] = useState<CouponDiscountType>(coupon?.discountType ?? "Percentage")
  const [discountValue, setDiscountValue] = useState(String(coupon?.discountValue ?? ""))
  const [maxDiscountAmount, setMaxDiscountAmount] = useState(coupon?.maxDiscountAmount != null ? String(coupon.maxDiscountAmount) : "")
  const [minCartAmount, setMinCartAmount] = useState(coupon?.minCartAmount != null ? String(coupon.minCartAmount) : "")
  const [scopeType, setScopeType] = useState<CouponScopeType>(coupon?.scopeType ?? "Cart")
  const [scopeReferenceId, setScopeReferenceId] = useState(coupon?.scopeReferenceId ?? "")
  const [startDate, setStartDate] = useState(coupon ? toLocalInput(coupon.startDate) : "")
  const [endDate, setEndDate] = useState(coupon ? toLocalInput(coupon.endDate) : "")
  const [totalUsageLimit, setTotalUsageLimit] = useState(coupon?.totalUsageLimit != null ? String(coupon.totalUsageLimit) : "")
  const [perUserUsageLimit, setPerUserUsageLimit] = useState(coupon?.perUserUsageLimit != null ? String(coupon.perUserUsageLimit) : "")

  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      const input = {
        discountType,
        discountValue: Number(discountValue),
        maxDiscountAmount: maxDiscountAmount ? Number(maxDiscountAmount) : null,
        minCartAmount: minCartAmount ? Number(minCartAmount) : null,
        scopeType,
        scopeReferenceId: scopeType === "Cart" ? null : scopeReferenceId || null,
        startDate: new Date(startDate).toISOString(),
        endDate: new Date(endDate).toISOString(),
        totalUsageLimit: totalUsageLimit ? Number(totalUsageLimit) : null,
        perUserUsageLimit: perUserUsageLimit ? Number(perUserUsageLimit) : null,
      }
      if (isEdit) {
        await pricingApi.updateCoupon(coupon.id, input)
        router.push("/admin/coupons")
      } else {
        await pricingApi.createCoupon({ ...input, code })
        router.push("/admin/coupons")
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kaydedilemedi.")
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div>
      <AdminPageHeader title={isEdit ? `Kupon: ${coupon.code}` : "Yeni Kupon"} />
      <AdminSection>
        <form onSubmit={handleSubmit} className="grid max-w-2xl grid-cols-1 gap-4 sm:grid-cols-2">
          <div className="sm:col-span-2">
            <Label>Kupon Kodu</Label>
            <Input required disabled={isEdit} value={code} onChange={(e) => setCode(e.target.value.toUpperCase())} placeholder="YAZINDIRIM10" />
          </div>
          <div>
            <Label>İndirim Tipi</Label>
            <Select value={discountType} onChange={(e) => setDiscountType(e.target.value as CouponDiscountType)}>
              <option value="Percentage">Yüzde (%)</option>
              <option value="FixedAmount">Sabit Tutar</option>
            </Select>
          </div>
          <div>
            <Label>İndirim Değeri</Label>
            <Input required type="number" min="0" step="0.01" value={discountValue} onChange={(e) => setDiscountValue(e.target.value)} />
          </div>
          <div>
            <Label>Maksimum İndirim (opsiyonel)</Label>
            <Input type="number" min="0" step="0.01" value={maxDiscountAmount} onChange={(e) => setMaxDiscountAmount(e.target.value)} />
          </div>
          <div>
            <Label>Minimum Sepet Tutarı (opsiyonel)</Label>
            <Input type="number" min="0" step="0.01" value={minCartAmount} onChange={(e) => setMinCartAmount(e.target.value)} />
          </div>
          <div>
            <Label>Kapsam</Label>
            <Select value={scopeType} onChange={(e) => setScopeType(e.target.value as CouponScopeType)}>
              <option value="Cart">Sepet Geneli</option>
              <option value="Category">Kategori</option>
              <option value="Product">Ürün</option>
            </Select>
          </div>
          {scopeType !== "Cart" && (
            <div>
              <Label>{scopeType === "Category" ? "Kategori Id" : "Ürün Id"}</Label>
              <Input required value={scopeReferenceId} onChange={(e) => setScopeReferenceId(e.target.value)} />
            </div>
          )}
          <div>
            <Label>Başlangıç Tarihi</Label>
            <Input required type="datetime-local" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
          </div>
          <div>
            <Label>Bitiş Tarihi</Label>
            <Input required type="datetime-local" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
          </div>
          <div>
            <Label>Toplam Kullanım Limiti (opsiyonel)</Label>
            <Input type="number" min="0" value={totalUsageLimit} onChange={(e) => setTotalUsageLimit(e.target.value)} />
          </div>
          <div>
            <Label>Kullanıcı Başına Limit (opsiyonel)</Label>
            <Input type="number" min="0" value={perUserUsageLimit} onChange={(e) => setPerUserUsageLimit(e.target.value)} />
          </div>
          {error && (
            <div className="sm:col-span-2">
              <FormMessage tone="error">{error}</FormMessage>
            </div>
          )}
          <div className="sm:col-span-2">
            <button type="submit" disabled={submitting} className="bg-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest text-background hover:opacity-90 disabled:opacity-50">
              {submitting ? "Kaydediliyor…" : isEdit ? "Kaydet" : "Kuponu Oluştur"}
            </button>
          </div>
        </form>
      </AdminSection>
    </div>
  )
}
