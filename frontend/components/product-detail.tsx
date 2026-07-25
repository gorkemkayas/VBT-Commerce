"use client"

import { useEffect, useMemo, useState } from "react"
import Image from "next/image"
import Link from "next/link"
import { Check, ChevronRight, Star } from "lucide-react"
import { useCart } from "@/components/cart-provider"
import { useAuth } from "@/lib/auth-context"
import { formatDate, formatPrice } from "@/lib/format"
import { getPriceOrNull } from "@/lib/store-catalog"
import { isColorAttributeName } from "@/lib/variant-utils"
import { getAvailableQuantity } from "@/lib/api/inventory"
import * as reviewsApi from "@/lib/api/reviews"
import { ApiError } from "@/lib/api/client"
import type { Product, Review, ReviewSummary, SellableItemType } from "@/lib/api/types"
import { Textarea } from "@/components/ui/textarea"
import { FormMessage } from "@/components/ui/form-message"

export function ProductDetail({
  product,
  categoryName,
  initialReviews,
  initialSummary,
}: {
  product: Product
  categoryName: string
  initialReviews: Review[]
  initialSummary: ReviewSummary
}) {
  const { addItem } = useCart()
  const { isAuthenticated } = useAuth()

  const isVariant = product.productType === "Variant"
  const [selectedVariantId, setSelectedVariantId] = useState<string>(
    isVariant ? product.variants.find((v) => v.isActive)?.id ?? "" : product.id,
  )
  const [activeImage, setActiveImage] = useState(0)
  const [price, setPrice] = useState<number | null>(null)
  const [stock, setStock] = useState<number | null>(null)
  const [loadingSelection, setLoadingSelection] = useState(true)
  const [added, setAdded] = useState(false)
  const [variantError, setVariantError] = useState(false)
  const [maxVariantPrice, setMaxVariantPrice] = useState<number | null>(null)

  const sellableItemType: SellableItemType = isVariant ? "Variant" : "Product"
  const sellableItemId = isVariant ? selectedVariantId : product.id

  // Varyantlar arasında en yüksek fiyatı "referans fiyat" olarak kullanıp, daha ucuz bir varyant
  // seçildiğinde indirim rozeti gösterebilmek için tüm aktif varyantların fiyatını tek seferde çekiyoruz.
  useEffect(() => {
    if (!isVariant) return
    let cancelled = false
    const activeVariants = product.variants.filter((v) => v.isActive)
    Promise.all(activeVariants.map((v) => getPriceOrNull("Variant", v.id))).then((prices) => {
      if (cancelled) return
      const numeric = prices.filter((p): p is number => p !== null)
      setMaxVariantPrice(numeric.length > 0 ? Math.max(...numeric) : null)
    })
    return () => {
      cancelled = true
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isVariant, product.id])

  const discountPercent =
    isVariant && price !== null && maxVariantPrice !== null && maxVariantPrice > price
      ? Math.round((1 - price / maxVariantPrice) * 100)
      : null

  useEffect(() => {
    if (!sellableItemId) {
      setLoadingSelection(false)
      return
    }
    let cancelled = false
    setLoadingSelection(true)
    Promise.all([getPriceOrNull(sellableItemType, sellableItemId), getAvailableQuantity(sellableItemId, sellableItemType)])
      .then(([p, s]) => {
        if (!cancelled) {
          setPrice(p)
          setStock(s)
        }
      })
      .catch(() => {
        if (!cancelled) {
          setPrice(null)
          setStock(null)
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingSelection(false)
      })
    return () => {
      cancelled = true
    }
  }, [sellableItemId, sellableItemType])

  const images = useMemo(() => {
    const variantImages = product.images.filter((img) => img.productVariantId === selectedVariantId)
    const base = variantImages.length > 0 ? variantImages : product.images.filter((img) => !img.productVariantId)
    const sorted = [...base].sort((a, b) => a.displayOrder - b.displayOrder)
    return sorted.length > 0 ? sorted.map((i) => i.url) : ["/placeholder.svg"]
  }, [product.images, selectedVariantId])

  useEffect(() => setActiveImage(0), [selectedVariantId])

  function handleAdd() {
    if (isVariant && !selectedVariantId) {
      setVariantError(true)
      return
    }
    const variant = product.variants.find((v) => v.id === selectedVariantId)
    addItem({
      sellableItemId,
      sellableItemType,
      meta: {
        productId: product.id,
        productName: product.name,
        productSlug: product.slug,
        image: images[0] ?? null,
        variantLabel: variant ? variant.optionValues.map((v) => v.value).join(" / ") : null,
        unitPrice: price,
      },
    })
    setAdded(true)
    setTimeout(() => setAdded(false), 2000)
  }

  const outOfStock = stock !== null && stock <= 0
  const otherImages = images.map((img, i) => ({ img, i })).filter(({ i }) => i !== activeImage)

  return (
    <div className="mx-auto max-w-6xl px-4 md:px-6">
      <nav className="flex items-center gap-2 py-6 text-[10px] font-medium uppercase tracking-widest text-muted-foreground">
        <Link href="/" className="transition-colors hover:text-foreground">
          Ana Sayfa
        </Link>
        <ChevronRight className="h-3 w-3" strokeWidth={1.5} />
        <Link href="/shop" className="transition-colors hover:text-foreground">
          Koleksiyon
        </Link>
        <ChevronRight className="h-3 w-3" strokeWidth={1.5} />
        <span className="text-foreground">{product.name}</span>
      </nav>

      <div className="grid grid-cols-1 gap-10 md:grid-cols-2 lg:gap-16">
        <div>
          <div className="group relative aspect-[3/4] overflow-hidden bg-secondary">
            <Image
              src={images[activeImage] || "/placeholder.svg"}
              alt={product.name}
              fill
              sizes="(max-width: 768px) 100vw, 50vw"
              className="object-cover transition-transform duration-500 ease-out group-hover:scale-105"
              priority
            />
            {discountPercent !== null && !loadingSelection && (
              <span className="absolute left-0 top-4 border border-foreground bg-foreground px-2.5 py-1 text-xs font-medium uppercase tracking-wider text-background">
                %{discountPercent} İndirim
              </span>
            )}
            {images.length > 1 && (
              <div className="absolute bottom-4 left-4 flex items-center gap-1.5">
                {images.map((_, i) => (
                  <button
                    key={i}
                    type="button"
                    onClick={() => setActiveImage(i)}
                    aria-label={`Görsel ${i + 1}`}
                    aria-pressed={activeImage === i}
                    className={`h-1.5 rounded-full transition-all ${
                      activeImage === i ? "w-5 bg-white" : "w-1.5 bg-white/50 hover:bg-white/80"
                    }`}
                  />
                ))}
              </div>
            )}
          </div>

          {otherImages.length > 0 && (
            <div className="mt-4 flex gap-3 overflow-x-auto pb-1">
              {otherImages.map(({ img, i }) => (
                <button
                  key={img + i}
                  type="button"
                  onClick={() => setActiveImage(i)}
                  aria-label="Görseli büyüt"
                  className="relative aspect-[3/4] w-20 shrink-0 overflow-hidden bg-secondary transition-opacity hover:opacity-80 sm:w-24"
                >
                  <Image src={img || "/placeholder.svg"} alt="" fill sizes="96px" className="object-cover" />
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="flex flex-col pt-2">
          <p className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">{categoryName}</p>
          <h1 className="mt-3 text-balance font-serif text-3xl font-medium tracking-tight md:text-4xl">{product.name}</h1>

          {initialSummary.totalCount > 0 && (
            <div className="mt-3 flex items-center gap-1.5">
              <Star className="h-4 w-4 fill-foreground text-foreground" strokeWidth={1.5} />
              <span className="text-sm font-medium">{initialSummary.averageRating.toFixed(1)}</span>
              <span className="text-xs text-muted-foreground">({initialSummary.totalCount} değerlendirme)</span>
            </div>
          )}

          <div className="mt-4 flex items-baseline gap-3">
            <p className="font-mono text-2xl">
              {loadingSelection ? "…" : price !== null ? formatPrice(price) : "Fiyat belirtilmemiş"}
            </p>
            {discountPercent !== null && maxVariantPrice !== null && !loadingSelection && (
              <p className="font-mono text-base text-muted-foreground line-through">{formatPrice(maxVariantPrice)}</p>
            )}
          </div>

          {product.description && <p className="mt-6 text-sm leading-relaxed text-muted-foreground">{product.description}</p>}

          {isVariant && product.variantAttributes.length > 0 && (
            <div className="mt-8">
              <div className="flex items-center justify-between">
                <span className="text-[10px] font-medium uppercase tracking-widest">Seçenek</span>
                {variantError && (
                  <span className="text-[10px] font-medium uppercase tracking-widest text-foreground">Lütfen seçenek seçin</span>
                )}
              </div>
              <div className="mt-3 flex flex-wrap gap-2">
                {product.variants
                  .filter((v) => v.isActive)
                  .map((v) => {
                    const isPureColor = v.optionValues.length > 0 && v.optionValues.every((o) => isColorAttributeName(o.attributeName))
                    const selected = selectedVariantId === v.id

                    if (isPureColor) {
                      return (
                        <button
                          key={v.id}
                          type="button"
                          onClick={() => {
                            setSelectedVariantId(v.id)
                            setVariantError(false)
                          }}
                          aria-label={v.optionValues.map((o) => o.value).join(" / ")}
                          aria-pressed={selected}
                          title={v.optionValues.map((o) => o.value).join(" / ")}
                          className={`h-9 w-9 border-2 transition-all ${selected ? "border-foreground" : "border-transparent hover:border-border"}`}
                        >
                          <span className="block h-full w-full border border-border" style={{ backgroundColor: v.optionValues[0].value }} />
                        </button>
                      )
                    }

                    return (
                      <button
                        key={v.id}
                        type="button"
                        onClick={() => {
                          setSelectedVariantId(v.id)
                          setVariantError(false)
                        }}
                        className={`flex min-w-12 items-center gap-2 border px-4 py-3 text-xs font-medium uppercase tracking-widest transition-colors ${
                          selected ? "border-foreground bg-foreground text-background" : "border-border hover:border-foreground"
                        }`}
                      >
                        {v.optionValues.map((o, i) =>
                          isColorAttributeName(o.attributeName) ? (
                            <span
                              key={i}
                              className="h-3.5 w-3.5 shrink-0 border border-border"
                              style={{ backgroundColor: o.value }}
                              title={o.value}
                            />
                          ) : (
                            <span key={i}>{o.value}</span>
                          ),
                        )}
                        {v.optionValues.length === 0 && v.sku}
                      </button>
                    )
                  })}
              </div>
            </div>
          )}

          {stock !== null && stock > 0 && stock <= 5 && (
            <p className="mt-4 text-xs text-muted-foreground">Son {stock} adet kaldı.</p>
          )}

          <button
            type="button"
            onClick={handleAdd}
            disabled={outOfStock || loadingSelection}
            className="mt-8 flex items-center justify-center gap-2 bg-foreground py-4 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-80 disabled:opacity-40"
          >
            {outOfStock ? (
              "Stokta Yok"
            ) : added ? (
              <>
                <Check className="h-4 w-4" strokeWidth={2} /> Sepete Eklendi
              </>
            ) : (
              "Sepete Ekle"
            )}
          </button>

          {product.attributes.length > 0 && (
            <div className="mt-10 border-t border-border pt-6">
              <h2 className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Ürün Detayları</h2>
              <ul className="mt-4 space-y-2">
                {product.attributes
                  .slice()
                  .sort((a, b) => a.displayOrder - b.displayOrder)
                  .map((attr) => (
                    <li key={attr.id} className="flex items-start gap-2 text-sm text-muted-foreground">
                      <span className="mt-2 h-px w-3 shrink-0 bg-border" />
                      <span>
                        <span className="text-foreground">{attr.name}:</span> {attr.value}
                      </span>
                    </li>
                  ))}
              </ul>
            </div>
          )}
        </div>
      </div>

      <ReviewsSection
        sellableItemId={product.id}
        sellableItemType="Product"
        initialReviews={initialReviews}
        initialSummary={initialSummary}
        canReview={isAuthenticated}
      />
    </div>
  )
}

function ReviewsSection({
  sellableItemId,
  sellableItemType,
  initialReviews,
  initialSummary,
  canReview,
}: {
  sellableItemId: string
  sellableItemType: SellableItemType
  initialReviews: Review[]
  initialSummary: ReviewSummary
  canReview: boolean
}) {
  const [reviews, setReviews] = useState(initialReviews)
  const [summary, setSummary] = useState(initialSummary)
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState("")
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      await reviewsApi.createMyReview({ sellableItemId, sellableItemType, rating, comment })
      setSuccess(true)
      setComment("")
      const [list, newSummary] = await Promise.all([
        reviewsApi.getProductReviews(sellableItemId, sellableItemType, 1, 10),
        reviewsApi.getProductReviewSummary(sellableItemId, sellableItemType),
      ])
      setReviews(list.items)
      setSummary(newSummary)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Yorum gönderilemedi.")
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="mx-auto mt-16 max-w-3xl border-t border-border pt-10">
      <h2 className="font-serif text-2xl font-medium tracking-tight">
        Değerlendirmeler {summary.totalCount > 0 && `(${summary.totalCount})`}
      </h2>

      {reviews.length === 0 ? (
        <p className="mt-4 text-sm text-muted-foreground">Bu ürün için henüz değerlendirme yapılmamış.</p>
      ) : (
        <ul className="mt-6 space-y-6">
          {reviews.map((review) => (
            <li key={review.id} className="border-b border-border pb-6">
              <div className="flex items-start justify-between gap-4">
                <div>
                  {review.reviewerDisplayName && (
                    <p className="text-sm font-semibold">{review.reviewerDisplayName}</p>
                  )}
                  <div className="mt-1.5 flex items-center gap-0.5">
                    {Array.from({ length: 5 }).map((_, i) => (
                      <Star
                        key={i}
                        className={`h-3.5 w-3.5 ${i < review.rating ? "fill-foreground text-foreground" : "text-border"}`}
                        strokeWidth={1.5}
                      />
                    ))}
                  </div>
                </div>
                <span className="shrink-0 text-xs text-muted-foreground">{formatDate(review.createdAt)}</span>
              </div>
              <p className="mt-3 text-sm leading-relaxed text-muted-foreground">{review.comment}</p>
            </li>
          ))}
        </ul>
      )}

      {canReview ? (
        <form onSubmit={handleSubmit} className="mt-8 space-y-4 border-t border-border pt-8">
          <h3 className="text-xs font-medium uppercase tracking-widest">Yorum Yaz</h3>
          <div className="flex items-center gap-1">
            {Array.from({ length: 5 }).map((_, i) => (
              <button key={i} type="button" onClick={() => setRating(i + 1)} aria-label={`${i + 1} yıldız`}>
                <Star
                  className={`h-6 w-6 transition-colors ${i < rating ? "fill-foreground text-foreground" : "text-border hover:text-muted-foreground"}`}
                  strokeWidth={1.5}
                />
              </button>
            ))}
          </div>
          <Textarea
            required
            minLength={3}
            rows={3}
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Bu ürün hakkındaki düşünceleriniz…"
          />
          {error && <FormMessage tone="error">{error}</FormMessage>}
          {success && <FormMessage tone="success">Yorumunuz eklendi.</FormMessage>}
          <button
            type="submit"
            disabled={submitting}
            className="border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest transition-colors hover:bg-foreground hover:text-background disabled:opacity-50"
          >
            {submitting ? "Gönderiliyor…" : "Yorumu Gönder"}
          </button>
        </form>
      ) : (
        <p className="mt-8 border-t border-border pt-8 text-sm text-muted-foreground">
          Yorum yazmak için{" "}
          <Link href="/login" className="font-medium text-foreground underline underline-offset-4">
            giriş yapın
          </Link>
          .
        </p>
      )}
    </section>
  )
}
