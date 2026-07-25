import { apiFetch } from "./client"
import type { PagedResult, Shipment, ShipmentStatus, ShippingCompany } from "./types"

// Herkese açık

export function getActiveShippingCompanies() {
  return apiFetch<ShippingCompany[]>("/api/shipping-companies", { auth: false })
}

// Admin — kargo firmaları

export function createShippingCompany(input: { name: string; fee: number }) {
  return apiFetch<string>("/api/admin/shipping-companies", { method: "POST", body: input })
}

export function updateShippingCompany(id: string, input: { name: string; fee: number }) {
  return apiFetch<void>(`/api/admin/shipping-companies/${id}`, { method: "PUT", body: input })
}

export function deactivateShippingCompany(id: string) {
  return apiFetch<void>(`/api/admin/shipping-companies/${id}/deactivate`, { method: "POST" })
}

export function getShippingCompanyById(id: string) {
  return apiFetch<ShippingCompany>(`/api/admin/shipping-companies/${id}`)
}

export function getShippingCompanies(pageNumber: number, pageSize: number) {
  return apiFetch<PagedResult<ShippingCompany>>("/api/admin/shipping-companies", { query: { pageNumber, pageSize } })
}

// Admin — gönderiler

export function getShipmentById(shipmentId: string) {
  return apiFetch<Shipment>(`/api/admin/shipments/${shipmentId}`)
}

export function getShipments(params: { status?: ShipmentStatus; pageNumber: number; pageSize: number }) {
  return apiFetch<PagedResult<Shipment>>("/api/admin/shipments", { query: params })
}

export function updateShipmentStatus(shipmentId: string, status: ShipmentStatus, trackingNumber?: string | null) {
  return apiFetch<void>(`/api/admin/shipments/${shipmentId}/status`, { method: "PUT", body: { status, trackingNumber } })
}
