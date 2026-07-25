"use client"

import { useRef, useState } from "react"
import Image from "next/image"
import Link from "next/link"
import { formatPrice } from "@/lib/format"

export type ProductCardData = {
  id: string
  slug: string
  name: string
  image: string | null
  images?: string[]
  price: number | null
  categoryName?: string
}

export function ProductCard({ product, className = "" }: { product: ProductCardData; className?: string }) {
  const gallery = product.images && product.images.length > 0 ? product.images : [product.image ?? "/placeholder.svg"]
  const [activeIndex, setActiveIndex] = useState(0)
  const rectRef = useRef<DOMRect | null>(null)

  function handleMouseEnter(e: React.MouseEvent<HTMLDivElement>) {
    if (gallery.length <= 1) return
    rectRef.current = e.currentTarget.getBoundingClientRect()
  }

  function handleMouseMove(e: React.MouseEvent<HTMLDivElement>) {
    if (gallery.length <= 1 || !rectRef.current) return
    const rect = rectRef.current
    const ratio = (e.clientX - rect.left) / rect.width
    const index = Math.min(gallery.length - 1, Math.max(0, Math.floor(ratio * gallery.length)))
    setActiveIndex(index)
  }

  return (
    <article className={`group ${className}`}>
      <Link href={`/product/${product.slug}`} className="block">
        <div
          className="relative aspect-[4/5] overflow-hidden bg-secondary"
          onMouseEnter={handleMouseEnter}
          onMouseMove={handleMouseMove}
          onMouseLeave={() => setActiveIndex(0)}
        >
          {/* All gallery images are stacked and preloaded up front; hovering only toggles opacity
              (no src swap), so switching is instant instead of waiting on a fresh image fetch. */}
          {gallery.map((src, i) => (
            <Image
              key={src + i}
              src={src || "/placeholder.svg"}
              alt={product.name}
              fill
              sizes="(max-width: 1024px) 50vw, 25vw"
              className={`object-cover transition-all duration-300 group-hover:scale-105 ${
                i === activeIndex ? "opacity-100" : "opacity-0"
              }`}
            />
          ))}
          {gallery.length > 1 && (
            <div className="absolute inset-x-0 bottom-2 flex items-center justify-center gap-1">
              {gallery.map((_, i) => (
                <span
                  key={i}
                  className={`h-1 rounded-full transition-all ${i === activeIndex ? "w-4 bg-white" : "w-1 bg-white/50"}`}
                />
              ))}
            </div>
          )}
        </div>
        <div className="space-y-1 px-1 py-4">
          {product.categoryName && (
            <p className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">{product.categoryName}</p>
          )}
          <div className="flex items-start justify-between gap-2">
            <h3 className="text-sm font-medium">{product.name}</h3>
            <p className="shrink-0 font-mono text-sm">{product.price !== null ? formatPrice(product.price) : "—"}</p>
          </div>
        </div>
      </Link>
    </article>
  )
}
