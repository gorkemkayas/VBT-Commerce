"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import * as catalogApi from "@/lib/api/catalog"
import type { CategoryTree, ProductType } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { FormMessage } from "@/components/ui/form-message"

function slugify(value: string) {
  return value
    .toLowerCase()
    .replace(/ğ/g, "g")
    .replace(/ü/g, "u")
    .replace(/ş/g, "s")
    .replace(/ı/g, "i")
    .replace(/ö/g, "o")
    .replace(/ç/g, "c")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "")
}

export function NewProductForm() {
  const router = useRouter()
  const [categories, setCategories] = useState<CategoryTree[]>([])
  const [name, setName] = useState("")
  const [slug, setSlug] = useState("")
  const [slugTouched, setSlugTouched] = useState(false)
  const [description, setDescription] = useState("")
  const [categoryId, setCategoryId] = useState("")
  const [productType, setProductType] = useState<ProductType>("Simple")
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    catalogApi.getCategoryTree(true).then((tree) => {
      setCategories(tree)
      const first = flatten(tree)[0]
      if (first) setCategoryId(first.node.id)
    })
  }, [])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      const id = await catalogApi.createProduct({ name, slug, description: description || null, categoryId, productType })
      router.push(`/admin/products/${id}`)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Ürün oluşturulamadı.")
      setSubmitting(false)
    }
  }

  return (
    <div>
      <AdminPageHeader title="Yeni Ürün" description="Temel ürün bilgilerini girin, sonraki adımda varyant/görsel/fiyat ekleyebilirsiniz." />
      <AdminSection>
        <form onSubmit={handleSubmit} className="max-w-xl space-y-4">
          <div>
            <Label>Ürün Adı</Label>
            <Input
              required
              value={name}
              onChange={(e) => {
                setName(e.target.value)
                if (!slugTouched) setSlug(slugify(e.target.value))
              }}
            />
          </div>
          <div>
            <Label>Slug (URL)</Label>
            <Input
              required
              value={slug}
              onChange={(e) => {
                setSlugTouched(true)
                setSlug(e.target.value)
              }}
            />
          </div>
          <div>
            <Label>Açıklama</Label>
            <Textarea rows={4} value={description} onChange={(e) => setDescription(e.target.value)} />
          </div>
          <div>
            <Label>Kategori</Label>
            <Select required value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
              {flatten(categories).map(({ node, depth }) => (
                <option key={node.id} value={node.id}>
                  {"—".repeat(depth)} {node.name}
                </option>
              ))}
            </Select>
          </div>
          <div>
            <Label>Ürün Tipi</Label>
            <Select value={productType} onChange={(e) => setProductType(e.target.value as ProductType)}>
              <option value="Simple">Basit (tek SKU)</option>
              <option value="Variant">Varyantlı (beden/renk vb.)</option>
            </Select>
          </div>
          {error && <FormMessage tone="error">{error}</FormMessage>}
          <button type="submit" disabled={submitting} className="bg-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest text-background hover:opacity-90 disabled:opacity-50">
            {submitting ? "Oluşturuluyor…" : "Ürünü Oluştur"}
          </button>
        </form>
      </AdminSection>
    </div>
  )
}

function flatten(nodes: CategoryTree[], depth = 0): { node: CategoryTree; depth: number }[] {
  return nodes.flatMap((node) => [{ node, depth }, ...flatten(node.children, depth + 1)])
}
