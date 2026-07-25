import { apiFetch } from "./client"
import type { Customer, CustomerListItem, GuestCustomer, PagedResult } from "./types"

// Kendi profilim

export function getMyProfile() {
  return apiFetch<Customer>("/api/customers/me")
}

export function createMyProfile(input: { phoneNumber?: string | null; dateOfBirth?: string | null }) {
  return apiFetch<string>("/api/customers/me", { method: "POST", body: input })
}

export function updateMyProfile(input: { phoneNumber?: string | null; dateOfBirth?: string | null }) {
  return apiFetch<void>("/api/customers/me", { method: "PUT", body: input })
}

export type AddressInput = {
  label: string
  recipientName: string
  phoneNumber: string
  country: string
  city: string
  district: string
  postalCode: string
  addressLine1: string
  addressLine2?: string | null
  isDefault: boolean
  isShippingAddress: boolean
  isBillingAddress: boolean
}

export function addMyAddress(input: AddressInput) {
  return apiFetch<string>("/api/customers/me/addresses", { method: "POST", body: input })
}

export function updateMyAddress(addressId: string, input: AddressInput) {
  return apiFetch<void>(`/api/customers/me/addresses/${addressId}`, { method: "PUT", body: input })
}

export function removeMyAddress(addressId: string) {
  return apiFetch<void>(`/api/customers/me/addresses/${addressId}`, { method: "DELETE" })
}

export function setDefaultMyAddress(addressId: string) {
  return apiFetch<void>(`/api/customers/me/addresses/${addressId}/set-default`, { method: "POST" })
}

// Misafir müşteri

export function createGuestCustomer(input: { firstName: string; lastName: string; email: string; phoneNumber: string }) {
  return apiFetch<string>("/api/guest-customers", { method: "POST", auth: false, body: input })
}

export function getGuestCustomer(guestCustomerId: string) {
  return apiFetch<GuestCustomer>(`/api/guest-customers/${guestCustomerId}`, { auth: false })
}

// Admin

export function getCustomerByUserId(userId: string) {
  return apiFetch<Customer>(`/api/customers/${userId}`)
}

export function getCustomers(pageNumber: number, pageSize: number) {
  return apiFetch<PagedResult<CustomerListItem>>("/api/customers", { query: { pageNumber, pageSize } })
}
