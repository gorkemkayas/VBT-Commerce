"use client"

import { useEffect, useState } from "react"
import { Star } from "lucide-react"
import * as reviewsApi from "@/lib/api/reviews"
import type { Review } from "@/lib/api/types"
import { formatDate } from "@/lib/format"
import { Textarea } from "@/components/ui/textarea"
import { EmptyState } from "@/components/ui/empty-state"
import { PageSpinner } from "@/components/ui/spinner"
import { FormMessage } from "@/components/ui/form-message"
import { ApiError } from "@/lib/api/client"

export function ReviewsTab() {
  const [reviews, setReviews] = useState<Review[]>([])
  const [loading, setLoading] = useState(true)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [editRating, setEditRating] = useState(5)
  const [editComment, setEditComment] = useState("")
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const result = await reviewsApi.getMyReviews(1, 50)
      setReviews(result.items)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  function startEdit(review: Review) {
    setEditingId(review.id)
    setEditRating(review.rating)
    setEditComment(review.comment)
  }

  async function handleUpdate(id: string) {
    setError(null)
    try {
      await reviewsApi.updateMyReview(id, { rating: editRating, comment: editComment })
      setEditingId(null)
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Güncellenemedi.")
    }
  }

  async function handleDelete(id: string) {
    if (!window.confirm("Bu yorumu silmek istediğinize emin misiniz?")) return
    await reviewsApi.deleteMyReview(id)
    await load()
  }

  if (loading) return <PageSpinner label="Yorumlar yükleniyor…" />

  if (reviews.length === 0) {
    return <EmptyState title="Henüz yorum yapmadınız" description="Satın aldığınız ürünler için görüşlerinizi paylaşabilirsiniz." />
  }

  return (
    <section className="space-y-6" aria-label="Yorumlarım">
      {error && <FormMessage tone="error">{error}</FormMessage>}
      {reviews.map((review) => (
        <article key={review.id} className="border border-border p-5">
          {editingId === review.id ? (
            <div className="space-y-3">
              <div className="flex items-center gap-1">
                {Array.from({ length: 5 }).map((_, i) => (
                  <button key={i} type="button" onClick={() => setEditRating(i + 1)} aria-label={`${i + 1} yıldız`}>
                    <Star className={`h-5 w-5 ${i < editRating ? "fill-foreground text-foreground" : "text-border"}`} strokeWidth={1.5} />
                  </button>
                ))}
              </div>
              <Textarea rows={3} value={editComment} onChange={(e) => setEditComment(e.target.value)} />
              <div className="flex gap-3">
                <button type="button" onClick={() => handleUpdate(review.id)} className="border border-foreground px-4 py-2 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background">
                  Kaydet
                </button>
                <button type="button" onClick={() => setEditingId(null)} className="px-4 py-2 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
                  Vazgeç
                </button>
              </div>
            </div>
          ) : (
            <>
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-0.5">
                  {Array.from({ length: 5 }).map((_, i) => (
                    <Star key={i} className={`h-3.5 w-3.5 ${i < review.rating ? "fill-foreground text-foreground" : "text-border"}`} strokeWidth={1.5} />
                  ))}
                </div>
                <span className="text-xs text-muted-foreground">{formatDate(review.createdAt)}</span>
              </div>
              <p className="mt-3 text-sm leading-relaxed">{review.comment}</p>
              <div className="mt-4 flex gap-4 text-xs font-medium uppercase tracking-widest">
                <button type="button" onClick={() => startEdit(review)} className="text-muted-foreground hover:text-foreground">
                  Düzenle
                </button>
                <button type="button" onClick={() => handleDelete(review.id)} className="text-muted-foreground hover:text-foreground">
                  Sil
                </button>
              </div>
            </>
          )}
        </article>
      ))}
    </section>
  )
}
