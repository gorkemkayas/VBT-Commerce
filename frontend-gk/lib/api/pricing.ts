import { apiFetch } from "./client"
import type { Coupon, CouponDiscountType, CouponScopeType, PagedResult, Price, PriceCalculationItem, PriceCalculationResult, SellableItemType } from "./types"

// Fiyat hesaplama (checkout önizlemesi)

export function calculateMyOrderPrice(items: PriceCalculationItem[], couponCodes: string[]) {
  return apiFetch<PriceCalculationResult>("/api/pricing/calculate/me", { method: "POST", body: { items, couponCodes } })
}

export function calculateGuestOrderPrice(guestCustomerId: string, items: PriceCalculationItem[], couponCodes: string[]) {
  return apiFetch<PriceCalculationResult>("/api/pricing/calculate/guest", {
    method: "POST",
    auth: false,
    body: { guestCustomerId, items, couponCodes },
  })
}

// Fiyatlar

export function getPrice(sellableItemType: SellableItemType, sellableItemId: string) {
  return apiFetch<Price>(`/api/prices/${sellableItemType}/${sellableItemId}`, { auth: false })
}

export function createPrice(input: { sellableItemId: string; sellableItemType: SellableItemType; amount: number }) {
  return apiFetch<string>("/api/admin/prices", { method: "POST", body: input })
}

export function updatePrice(priceId: string, amount: number) {
  return apiFetch<void>(`/api/admin/prices/${priceId}`, { method: "PUT", body: { amount } })
}

// Vergi oranı

export function getTaxRate() {
  return apiFetch<number>("/api/tax-rate", { auth: false })
}

export function updateTaxRate(rate: number) {
  return apiFetch<void>("/api/admin/tax-rate", { method: "PUT", body: { rate } })
}

// Kuponlar (admin)

export type CouponInput = {
  discountType: CouponDiscountType
  discountValue: number
  maxDiscountAmount?: number | null
  minCartAmount?: number | null
  scopeType: CouponScopeType
  scopeReferenceId?: string | null
  startDate: string
  endDate: string
  totalUsageLimit?: number | null
  perUserUsageLimit?: number | null
}

export function createCoupon(input: CouponInput & { code: string }) {
  return apiFetch<string>("/api/admin/coupons", { method: "POST", body: input })
}

export function updateCoupon(couponId: string, input: CouponInput) {
  return apiFetch<void>(`/api/admin/coupons/${couponId}`, { method: "PUT", body: input })
}

export function deactivateCoupon(couponId: string) {
  return apiFetch<void>(`/api/admin/coupons/${couponId}/deactivate`, { method: "POST" })
}

export function getCouponByCode(code: string) {
  return apiFetch<Coupon>(`/api/admin/coupons/${code}`)
}

export function getCoupons(pageNumber: number, pageSize: number) {
  return apiFetch<PagedResult<Coupon>>("/api/admin/coupons", { query: { pageNumber, pageSize } })
}

// Kuponlar (mağaza)

export function getActiveCoupons() {
  return apiFetch<Coupon[]>("/api/coupons/active", { auth: false })
}
