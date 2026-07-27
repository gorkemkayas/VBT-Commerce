"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import * as catalogApi from "@/lib/api/catalog"
import * as pricingApi from "@/lib/api/pricing"
import { ApiError } from "@/lib/api/client"
import type { CategoryTree, Product } from "@/lib/api/types"
import { AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { Badge } from "@/components/ui/badge"
import { FormMessage } from "@/components/ui/form-message"
import { formatPrice } from "@/lib/format"

export function GeneralInfoSection({
  product,
  categories,
  onSaved,
}: {
  product: Product
  categories: CategoryTree[]
  onSaved: () => void
}) {
  const router = useRouter()
  const [name, setName] = useState(product.name)
  const [slug, setSlug] = useState(product.slug)
  const [description, setDescription] = useState(product.description ?? "")
  const [categoryId, setCategoryId] = useState(product.categoryId)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      await catalogApi.updateProduct(product.id, { name, slug, description: description || null, categoryId })
      setSuccess(true)
      onSaved()
      setTimeout(() => setSuccess(false), 2000)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  async function handleActivate() {
    await catalogApi.activateProduct(product.id)
    onSaved()
  }

  async function handleDelete() {
    if (!window.confirm(`"${product.name}" ürününü silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`)) return
    await catalogApi.deleteProduct(product.id)
    router.push("/admin/products")
  }

  return (
    <AdminSection
      title="Genel Bilgiler"
      action={
        <div className="flex items-center gap-3">
          <Badge tone={product.isActive ? "success" : "neutral"}>{product.isActive ? "Aktif" : "Pasif"}</Badge>
          {!product.isActive && (
            <button type="button" onClick={handleActivate} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
              Yayına Al
            </button>
          )}
          <button type="button" onClick={handleDelete} className="text-xs font-medium uppercase tracking-widest text-red-600 hover:text-red-700">
            Sil
          </button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <Label>Ürün Adı</Label>
          <Input required value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div>
          <Label>Slug</Label>
          <Input required value={slug} onChange={(e) => setSlug(e.target.value)} />
        </div>
        <div className="sm:col-span-2">
          <Label>Açıklama</Label>
          <Textarea rows={3} value={description} onChange={(e) => setDescription(e.target.value)} />
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
          <p className="py-3 text-sm text-muted-foreground">{product.productType === "Simple" ? "Basit (tek SKU)" : "Varyantlı"}</p>
        </div>
        {error && (
          <div className="sm:col-span-2">
            <FormMessage tone="error">{error}</FormMessage>
          </div>
        )}
        {success && (
          <div className="sm:col-span-2">
            <FormMessage tone="success">Kaydedildi.</FormMessage>
          </div>
        )}
        <div className="sm:col-span-2">
          <button type="submit" disabled={saving} className="border border-foreground px-6 py-2.5 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
            {saving ? "Kaydediliyor…" : "Kaydet"}
          </button>
        </div>
      </form>
    </AdminSection>
  )
}

function flatten(nodes: CategoryTree[], depth = 0): { node: CategoryTree; depth: number }[] {
  return nodes.flatMap((node) => [{ node, depth }, ...flatten(node.children, depth + 1)])
}

export function PriceSection({ product }: { product: Product }) {
  const targets =
    product.productType === "Simple"
      ? [{ id: product.id, label: "Ürün Fiyatı", type: "Product" as const }]
      : product.variants.map((v) => ({ id: v.id, label: v.optionValues.map((o) => o.value).join(" / ") || v.sku, type: "Variant" as const }))

  return (
    <AdminSection title="Fiyat">
      {targets.length === 0 ? (
        <p className="text-sm text-muted-foreground">Fiyat tanımlamak için önce varyant ekleyin.</p>
      ) : (
        <div className="space-y-4">
          {targets.map((t) => (
            <PriceRow key={t.id} sellableItemId={t.id} sellableItemType={t.type} label={t.label} />
          ))}
        </div>
      )}
    </AdminSection>
  )
}

function PriceRow({ sellableItemId, sellableItemType, label }: { sellableItemId: string; sellableItemType: "Product" | "Variant"; label: string }) {
  const [priceId, setPriceId] = useState<string | null>(null)
  const [amount, setAmount] = useState("")
  const [loading, setLoading] = useState(true)
  const [editing, setEditing] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const price = await pricingApi.getPrice(sellableItemType, sellableItemId).catch((err) => {
        if (err instanceof ApiError && err.status === 404) return null
        throw err
      })
      setPriceId(price?.id ?? null)
      setAmount(price ? String(price.amount) : "")
      setEditing(price === null)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [sellableItemId])

  async function handleSave() {
    setError(null)
    setSaving(true)
    try {
      const numeric = Number(amount)
      if (priceId) {
        await pricingApi.updatePrice(priceId, numeric)
      } else {
        const id = await pricingApi.createPrice({ sellableItemId, sellableItemType, amount: numeric })
        setPriceId(id)
      }
      setEditing(false)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Fiyat kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <p className="text-xs text-muted-foreground">Yükleniyor…</p>

  return (
    <div className="flex flex-wrap items-center justify-between gap-3 border border-border px-4 py-3">
      <span className="text-sm">{label}</span>
      {editing ? (
        <div className="flex items-center gap-2">
          <Input type="number" min="0" step="0.01" value={amount} onChange={(e) => setAmount(e.target.value)} className="w-32" />
          <button type="button" onClick={handleSave} disabled={saving} className="border border-foreground px-3 py-2 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
            Kaydet
          </button>
          {error && <FormMessage tone="error">{error}</FormMessage>}
        </div>
      ) : (
        <div className="flex items-center gap-3">
          <span className="font-mono text-sm">{formatPrice(Number(amount))}</span>
          <button type="button" onClick={() => setEditing(true)} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
            Düzenle
          </button>
        </div>
      )}
    </div>
  )
}
