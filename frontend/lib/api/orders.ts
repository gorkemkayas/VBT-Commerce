import { apiFetch } from "./client"
import type { Order, OrderStatus, PagedResult, ShipmentTracking } from "./types"

export type PlaceMyOrderInput = {
  addressId: string
  billingAddressId?: string | null
  shippingCompanyId: string
  couponCodes: string[]
  cardHolderName: string
  cardNumber: string
  cardExpireMonth: string
  cardExpireYear: string
  cardCvc: string
  buyerIdentityNumber: string
}

export function placeMyOrder(input: PlaceMyOrderInput) {
  return apiFetch<string>("/api/orders/me", { method: "POST", body: input })
}

export function getMyOrder(orderId: string) {
  return apiFetch<Order>(`/api/orders/me/${orderId}`)
}

export function getMyOrders(pageNumber: number, pageSize: number) {
  return apiFetch<PagedResult<Order>>("/api/orders/me", { query: { pageNumber, pageSize } })
}

export function cancelMyOrder(orderId: string) {
  return apiFetch<void>(`/api/orders/me/${orderId}/cancel`, { method: "POST" })
}

export function getMyOrderShipment(orderId: string) {
  return apiFetch<ShipmentTracking>(`/api/orders/me/${orderId}/shipment`)
}

export type PlaceGuestOrderInput = {
  guestCustomerId: string
  anonymousId: string
  shippingCompanyId: string
  couponCodes: string[]
  recipientName: string
  phoneNumber: string
  country: string
  city: string
  district: string
  postalCode: string
  addressLine1: string
  addressLine2?: string | null
  billingRecipientName?: string | null
  billingPhoneNumber?: string | null
  billingCountry?: string | null
  billingCity?: string | null
  billingDistrict?: string | null
  billingPostalCode?: string | null
  billingAddressLine1?: string | null
  billingAddressLine2?: string | null
  cardHolderName: string
  cardNumber: string
  cardExpireMonth: string
  cardExpireYear: string
  cardCvc: string
  buyerIdentityNumber: string
}

export function placeGuestOrder(input: PlaceGuestOrderInput) {
  return apiFetch<string>("/api/orders/guest", { method: "POST", auth: false, body: input })
}

export function getGuestOrder(guestCustomerId: string, orderId: string) {
  return apiFetch<Order>(`/api/orders/guest/${guestCustomerId}/${orderId}`, { auth: false })
}

// Admin

export function getAdminOrder(orderId: string) {
  return apiFetch<Order>(`/api/admin/orders/${orderId}`)
}

export function getAdminOrders(params: { status?: OrderStatus; pageNumber: number; pageSize: number }) {
  return apiFetch<PagedResult<Order>>("/api/admin/orders", { query: params })
}

export function cancelAdminOrder(orderId: string, reason?: string | null) {
  return apiFetch<void>(`/api/admin/orders/${orderId}/cancel`, { method: "POST", body: { reason } })
}
