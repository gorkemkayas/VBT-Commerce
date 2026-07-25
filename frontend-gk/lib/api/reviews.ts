import { apiFetch } from "./client"
import type { PagedResult, Review, ReviewSummary, SellableItemType } from "./types"

// Herkese açık

export function getProductReviews(sellableItemId: string, sellableItemType: SellableItemType, pageNumber: number, pageSize: number) {
  return apiFetch<PagedResult<Review>>("/api/reviews", { auth: false, query: { sellableItemId, sellableItemType, pageNumber, pageSize } })
}

export function getProductReviewSummary(sellableItemId: string, sellableItemType: SellableItemType) {
  return apiFetch<ReviewSummary>("/api/reviews/summary", { auth: false, query: { sellableItemId, sellableItemType } })
}

// Kendi yorumlarım

export function getMyReviews(pageNumber: number, pageSize: number) {
  return apiFetch<PagedResult<Review>>("/api/reviews/me", { query: { pageNumber, pageSize } })
}

export function createMyReview(input: { sellableItemId: string; sellableItemType: SellableItemType; rating: number; comment: string }) {
  return apiFetch<string>("/api/reviews/me", { method: "POST", body: input })
}

export function updateMyReview(reviewId: string, input: { rating: number; comment: string }) {
  return apiFetch<void>(`/api/reviews/me/${reviewId}`, { method: "PUT", body: input })
}

export function deleteMyReview(reviewId: string) {
  return apiFetch<void>(`/api/reviews/me/${reviewId}`, { method: "DELETE" })
}
