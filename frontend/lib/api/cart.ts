import { apiFetch } from "./client"
import type { Cart, SellableItemType } from "./types"

// Anonim (misafir) sepet

export function getAnonymousCart(anonymousId: string) {
  return apiFetch<Cart>(`/api/carts/anonymous/${anonymousId}`, { auth: false })
}

export function addAnonymousCartItem(anonymousId: string, sellableItemId: string, sellableItemType: SellableItemType, quantity: number) {
  return apiFetch<string>(`/api/carts/anonymous/${anonymousId}/items`, {
    method: "POST",
    auth: false,
    body: { sellableItemId, sellableItemType, quantity },
  })
}

export function updateAnonymousCartItem(anonymousId: string, itemId: string, quantity: number) {
  return apiFetch<void>(`/api/carts/anonymous/${anonymousId}/items/${itemId}`, {
    method: "PUT",
    auth: false,
    body: { quantity },
  })
}

export function removeAnonymousCartItem(anonymousId: string, itemId: string) {
  return apiFetch<void>(`/api/carts/anonymous/${anonymousId}/items/${itemId}`, { method: "DELETE", auth: false })
}

export function clearAnonymousCart(anonymousId: string) {
  return apiFetch<void>(`/api/carts/anonymous/${anonymousId}`, { method: "DELETE", auth: false })
}

// Üye sepeti

export function getMyCart() {
  return apiFetch<Cart>("/api/carts/me")
}

export function addMyCartItem(sellableItemId: string, sellableItemType: SellableItemType, quantity: number) {
  return apiFetch<string>("/api/carts/me/items", { method: "POST", body: { sellableItemId, sellableItemType, quantity } })
}

export function updateMyCartItem(itemId: string, quantity: number) {
  return apiFetch<void>(`/api/carts/me/items/${itemId}`, { method: "PUT", body: { quantity } })
}

export function removeMyCartItem(itemId: string) {
  return apiFetch<void>(`/api/carts/me/items/${itemId}`, { method: "DELETE" })
}

export function clearMyCart() {
  return apiFetch<void>("/api/carts/me", { method: "DELETE" })
}
