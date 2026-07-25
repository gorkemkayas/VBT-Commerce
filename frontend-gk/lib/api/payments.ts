import { apiFetch } from "./client"
import type { PagedResult, Payment, PaymentStatus } from "./types"

export function getPaymentById(paymentId: string) {
  return apiFetch<Payment>(`/api/admin/payments/${paymentId}`)
}

export function getPayments(params: { status?: PaymentStatus; pageNumber: number; pageSize: number }) {
  return apiFetch<PagedResult<Payment>>("/api/admin/payments", { query: params })
}
