import { apiFetch } from "./client"
import type { PagedResult, SellableItemType, StockItem, StockReservation } from "./types"

export function getAvailableQuantity(sellableItemId: string, sellableItemType: SellableItemType) {
  return apiFetch<number>(`/api/stock-reservations/available-quantity/${sellableItemId}`, {
    auth: false,
    query: { sellableItemType },
  })
}

export function getStockItems(params: { pageNumber: number; pageSize: number; sellableItemType?: SellableItemType }) {
  return apiFetch<PagedResult<StockItem>>("/api/stock-items", { query: params })
}

export function getStockItemById(stockItemId: string) {
  return apiFetch<StockItem>(`/api/stock-items/${stockItemId}`)
}

export function getStockItemBySellableItem(sellableItemId: string, sellableItemType: SellableItemType) {
  return apiFetch<StockItem>(`/api/stock-items/by-sellable-item/${sellableItemId}`, { query: { sellableItemType } })
}

export function createStockItem(input: { sellableItemId: string; sellableItemType: SellableItemType; initialQuantity: number }) {
  return apiFetch<string>("/api/stock-items", { method: "POST", body: input })
}

export function increaseStock(stockItemId: string, quantity: number) {
  return apiFetch<void>(`/api/stock-items/${stockItemId}/increase`, { method: "POST", body: { quantity } })
}

export function decreaseStock(stockItemId: string, quantity: number) {
  return apiFetch<void>(`/api/stock-items/${stockItemId}/decrease`, { method: "POST", body: { quantity } })
}

export function getStockReservations(params: {
  pageNumber: number
  pageSize: number
  sellableItemId?: string
  sellableItemType?: SellableItemType
  isConfirmed?: boolean
  isReleased?: boolean
}) {
  return apiFetch<PagedResult<StockReservation>>("/api/stock-reservations", { query: params })
}

// Rezervasyonlar — destek/hata giderme amaçlı manuel işlemler

export function releaseReservationsByReference(referenceId: string) {
  return apiFetch<void>(`/api/stock-reservations/${referenceId}/release`, { method: "POST" })
}

export function confirmReservationsByReference(referenceId: string) {
  return apiFetch<void>(`/api/stock-reservations/${referenceId}/confirm`, { method: "POST" })
}
