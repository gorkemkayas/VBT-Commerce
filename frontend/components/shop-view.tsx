"use client"

import { useEffect, useMemo, useState } from "react"
import { ProductCard } from "@/components/product-card"
import type { CategoryTree, ProductListItem } from "@/lib/api/types"
import { Spinner } from "@/components/ui/spinner"

const sorts = [
  { value: "featured", label: "Öne Çıkan" },
  { value: "price-asc", label: "Fiyat: Artan" },
  { value: "price-desc", label: "Fiyat: Azalan" },
] as const

type ProductWithPrice = ProductListItem & { price: number | null }

export function ShopView({ categories, initialCategoryId }: { categories: CategoryTree[]; initialCategoryId?: string }) {
  const [categoryId, setCategoryId] = useState<string>(initialCategoryId ?? "")
  const [searchTerm, setSearchTerm] = useState("")
  const [sort, setSort] = useState<string>("featured")
  const [products, setProducts] = useState<ProductWithPrice[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    const timeout = setTimeout(async () => {
      try {
        const query = new URLSearchParams()
        if (categoryId) query.set("categoryId", categoryId)
        if (searchTerm) query.set("searchTerm", searchTerm)
        const res = await fetch(`/api/shop-products?${query.toString()}`)
        const { items } = (await res.json()) as { items: ProductWithPrice[] }
        if (!cancelled) setProducts(items)
      } finally {
        if (!cancelled) setLoading(false)
      }
    }, 250)
    return () => {
      cancelled = true
      clearTimeout(timeout)
    }
  }, [categoryId, searchTerm])

  const sorted = useMemo(() => {
    const list = [...products]
    if (sort === "price-asc") list.sort((a, b) => (a.price ?? 0) - (b.price ?? 0))
    if (sort === "price-desc") list.sort((a, b) => (b.price ?? 0) - (a.price ?? 0))
    return list
  }, [products, sort])

  const categoryOptions = flattenWithDepth(categories)
  const categoryNameById = new Map(categoryOptions.map((c) => [c.node.id, c.node.name]))

  return (
    <div className="mx-auto max-w-7xl px-4 md:px-6">
      <div className="border-b border-border py-10 md:py-14">
        <p className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Koleksiyon</p>
        <h1 className="mt-2 font-serif text-4xl font-medium tracking-tight md:text-6xl">Tüm Ürünler</h1>
      </div>

      <div className="border-b border-border py-6">
        <span className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Kategori</span>
        <div className="mt-3 flex flex-wrap gap-2">
          <button
            type="button"
            onClick={() => setCategoryId("")}
            className={`border px-4 py-2 text-xs font-medium uppercase tracking-widest transition-colors ${
              categoryId === ""
                ? "border-foreground bg-foreground text-background"
                : "border-border text-muted-foreground hover:border-foreground hover:text-foreground"
            }`}
          >
            Tümü
          </button>
          {categoryOptions.map(({ node, depth }) => (
            <button
              key={node.id}
              type="button"
              onClick={() => setCategoryId(node.id)}
              className={`inline-flex items-center gap-1 border px-4 py-2 text-xs font-medium uppercase tracking-widest transition-colors ${
                categoryId === node.id
                  ? "border-foreground bg-foreground text-background"
                  : "border-border text-muted-foreground hover:border-foreground hover:text-foreground"
              }`}
            >
              {depth > 0 && <span className="opacity-50">›</span>}
              {node.name}
            </button>
          ))}
        </div>
      </div>

      <div className="flex flex-col gap-4 border-b border-border py-6 sm:flex-row sm:items-center sm:justify-between">
        <input
          type="search"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          placeholder="Ürün ara…"
          aria-label="Ürün ara"
          className="w-full border border-border bg-background px-3 py-2 text-xs outline-none focus:border-foreground sm:w-64"
        />
        <div className="flex items-center gap-3">
          <span className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Sırala</span>
          <select
            value={sort}
            onChange={(e) => setSort(e.target.value)}
            className="border border-border bg-background px-3 py-2 text-xs font-medium uppercase tracking-widest focus:outline-none focus:ring-1 focus:ring-ring"
            aria-label="Sıralama"
          >
            {sorts.map((s) => (
              <option key={s.value} value={s.value}>
                {s.label}
              </option>
            ))}
          </select>
        </div>
      </div>

      <p className="flex items-center gap-2 py-4 text-[10px] font-medium uppercase tracking-widest text-muted-foreground">
        {loading && <Spinner className="h-3 w-3" />}
        {sorted.length} ürün
      </p>

      {!loading && sorted.length === 0 ? (
        <div className="flex min-h-64 items-center justify-center border border-border">
          <p className="text-sm text-muted-foreground">Bu filtrelere uygun ürün bulunamadı.</p>
        </div>
      ) : (
        <div className="grid grid-cols-2 gap-x-6 gap-y-10 pb-16 lg:grid-cols-4 lg:gap-x-8">
          {sorted.map((product) => (
            <ProductCard
              key={product.id}
              product={{
                id: product.id,
                slug: product.slug,
                name: product.name,
                image: product.primaryImageUrl,
                price: product.price,
                categoryName: categoryNameById.get(product.categoryId),
              }}
            />
          ))}
        </div>
      )}
    </div>
  )
}

function flattenWithDepth(nodes: CategoryTree[], depth = 0): { node: CategoryTree; depth: number }[] {
  return nodes.flatMap((node) => [{ node, depth }, ...flattenWithDepth(node.children, depth + 1)])
}
