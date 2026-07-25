"use client"

import { useEffect, useRef, useState } from "react"
import Image from "next/image"
import { X } from "lucide-react"
import * as catalogApi from "@/lib/api/catalog"
import type { ProductListItem } from "@/lib/api/types"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"

export function ProductPicker({
  selectedProduct,
  previewImageUrl,
  onSelect,
  onClear,
}: {
  selectedProduct: ProductListItem | null
  previewImageUrl?: string | null
  onSelect: (product: ProductListItem) => void
  onClear: () => void
}) {
  const [query, setQuery] = useState("")
  const [results, setResults] = useState<ProductListItem[]>([])
  const [open, setOpen] = useState(false)
  const [searching, setSearching] = useState(false)
  const containerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!query.trim()) {
      setResults([])
      return
    }
    let cancelled = false
    setSearching(true)
    const t = setTimeout(async () => {
      try {
        const result = await catalogApi.getProducts({ pageNumber: 1, pageSize: 8, searchTerm: query })
        if (!cancelled) setResults(result.items)
      } finally {
        if (!cancelled) setSearching(false)
      }
    }, 250)
    return () => {
      cancelled = true
      clearTimeout(t)
    }
  }, [query])

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) setOpen(false)
    }
    document.addEventListener("mousedown", handleClickOutside)
    return () => document.removeEventListener("mousedown", handleClickOutside)
  }, [])

  if (selectedProduct) {
    return (
      <div className="flex items-center gap-3 border border-border bg-secondary/40 px-3 py-2.5">
        <div className="relative h-10 w-10 shrink-0 overflow-hidden bg-secondary">
          <Image
            key={previewImageUrl || selectedProduct.primaryImageUrl || "placeholder"}
            src={previewImageUrl || selectedProduct.primaryImageUrl || "/placeholder.svg"}
            alt=""
            fill
            sizes="40px"
            className="object-cover"
          />
        </div>
        <div className="min-w-0 flex-1">
          <p className="truncate text-sm font-medium">{selectedProduct.name}</p>
          <p className="text-xs text-muted-foreground">{selectedProduct.productType === "Variant" ? "Varyantlı ürün" : "Basit ürün"}</p>
        </div>
        <button type="button" onClick={onClear} aria-label="Seçimi kaldır" className="shrink-0 p-1 text-muted-foreground hover:text-foreground">
          <X className="h-4 w-4" strokeWidth={1.5} />
        </button>
      </div>
    )
  }

  return (
    <div ref={containerRef} className="relative">
      <Input
        value={query}
        onChange={(e) => {
          setQuery(e.target.value)
          setOpen(true)
        }}
        onFocus={() => setOpen(true)}
        placeholder="Ürün adıyla ara…"
      />
      {open && query.trim() && (
        <div className="absolute z-20 mt-1 max-h-72 w-full overflow-auto border border-border bg-background shadow-[0_8px_24px_rgba(0,0,0,0.08)]">
          {searching ? (
            <p className="px-4 py-3 text-sm text-muted-foreground">Aranıyor…</p>
          ) : results.length === 0 ? (
            <p className="px-4 py-3 text-sm text-muted-foreground">Ürün bulunamadı.</p>
          ) : (
            results.map((product) => (
              <button
                key={product.id}
                type="button"
                onClick={() => {
                  onSelect(product)
                  setQuery("")
                  setResults([])
                  setOpen(false)
                }}
                className="flex w-full items-center gap-3 px-3 py-2.5 text-left transition-colors hover:bg-secondary"
              >
                <div className="relative h-9 w-9 shrink-0 overflow-hidden bg-secondary">
                  <Image src={product.primaryImageUrl || "/placeholder.svg"} alt="" fill sizes="36px" className="object-cover" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm">{product.name}</p>
                </div>
                {product.productType === "Variant" && (
                  <Badge tone="neutral" className="shrink-0">
                    Varyantlı
                  </Badge>
                )}
              </button>
            ))
          )}
        </div>
      )}
    </div>
  )
}
