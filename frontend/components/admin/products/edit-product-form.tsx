"use client"

import { useCallback, useEffect, useState } from "react"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import * as catalogApi from "@/lib/api/catalog"
import type { CategoryTree, Product } from "@/lib/api/types"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"
import { GeneralInfoSection, PriceSection } from "@/components/admin/products/product-overview-section"
import { AttributesSection, ImagesSection, VariantAttributesSection, VariantsSection } from "@/components/admin/products/product-attributes-section"

export function EditProductForm({ productId }: { productId: string }) {
  const [product, setProduct] = useState<Product | null>(null)
  const [categories, setCategories] = useState<CategoryTree[]>([])
  const [loading, setLoading] = useState(true)
  const [notFound, setNotFound] = useState(false)

  const load = useCallback(async () => {
    try {
      const [p, cats] = await Promise.all([catalogApi.getProductById(productId), catalogApi.getCategoryTree(true)])
      setProduct(p)
      setCategories(cats)
    } catch {
      setNotFound(true)
    } finally {
      setLoading(false)
    }
  }, [productId])

  useEffect(() => {
    load()
  }, [load])

  if (loading) return <PageSpinner label="Ürün yükleniyor…" />
  if (notFound || !product) return <EmptyState title="Ürün bulunamadı" />

  return (
    <div>
      <Link href="/admin/products" className="mb-4 flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
        <ChevronLeft className="h-3.5 w-3.5" strokeWidth={1.5} />
        Ürünlere Dön
      </Link>
      <div className="space-y-6">
        <GeneralInfoSection product={product} categories={categories} onSaved={load} />
        <PriceSection product={product} />
        <AttributesSection product={product} onSaved={load} />
        <VariantAttributesSection product={product} onSaved={load} />
        <VariantsSection product={product} onSaved={load} />
        <ImagesSection product={product} onSaved={load} />
      </div>
    </div>
  )
}
