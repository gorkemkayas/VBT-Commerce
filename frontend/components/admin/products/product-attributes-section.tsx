"use client"

import { useState } from "react"
import * as catalogApi from "@/lib/api/catalog"
import { ApiError } from "@/lib/api/client"
import type { Product } from "@/lib/api/types"
import { AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { FormMessage } from "@/components/ui/form-message"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { isColorAttributeName, toHexColorOrDefault } from "@/lib/variant-utils"
import { X } from "lucide-react"

export function AttributesSection({ product, onSaved }: { product: Product; onSaved: () => void }) {
  const [name, setName] = useState("")
  const [value, setValue] = useState("")
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      await catalogApi.addProductAttribute(product.id, { name, value, displayOrder: product.attributes.length })
      setName("")
      setValue("")
      onSaved()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Eklenemedi.")
    } finally {
      setSubmitting(false)
    }
  }

  async function handleRemove(attributeId: string) {
    await catalogApi.removeProductAttribute(product.id, attributeId)
    onSaved()
  }

  return (
    <AdminSection title="Ürün Öznitelikleri">
      {product.attributes.length > 0 && (
        <Table>
          <THead>
            <TR>
              <TH>Ad</TH>
              <TH>Değer</TH>
              <TH></TH>
            </TR>
          </THead>
          <TBody>
            {product.attributes
              .slice()
              .sort((a, b) => a.displayOrder - b.displayOrder)
              .map((attr) => (
                <TR key={attr.id}>
                  <TD>{attr.name}</TD>
                  <TD className="text-muted-foreground">{attr.value}</TD>
                  <TD className="text-right">
                    <button type="button" onClick={() => handleRemove(attr.id)} aria-label="Kaldır" className="text-muted-foreground hover:text-foreground">
                      <X className="h-4 w-4" strokeWidth={1.5} />
                    </button>
                  </TD>
                </TR>
              ))}
          </TBody>
        </Table>
      )}
      <form onSubmit={handleAdd} className="mt-4 flex flex-wrap items-end gap-3">
        <div className="flex-1">
          <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">Ad (örn. Kumaş)</label>
          <Input required value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div className="flex-1">
          <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">Değer (örn. %100 Pamuk)</label>
          <Input required value={value} onChange={(e) => setValue(e.target.value)} />
        </div>
        <button type="submit" disabled={submitting} className="border border-foreground px-4 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
          Ekle
        </button>
      </form>
      {error && (
        <div className="mt-3">
          <FormMessage tone="error">{error}</FormMessage>
        </div>
      )}
    </AdminSection>
  )
}

export function VariantAttributesSection({ product, onSaved }: { product: Product; onSaved: () => void }) {
  const [name, setName] = useState("")
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  if (product.productType !== "Variant") return null

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      await catalogApi.addProductVariantAttribute(product.id, { name, displayOrder: product.variantAttributes.length })
      setName("")
      onSaved()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Eklenemedi.")
    } finally {
      setSubmitting(false)
    }
  }

  async function handleRemove(id: string) {
    await catalogApi.removeProductVariantAttribute(product.id, id)
    onSaved()
  }

  return (
    <AdminSection title="Varyant Eksenleri" action={<span className="text-xs text-muted-foreground">örn. Beden, Renk</span>}>
      {product.variantAttributes.length > 0 && (
        <div className="mb-4 flex flex-wrap gap-2">
          {product.variantAttributes.map((va) => (
            <span key={va.id} className="flex items-center gap-1.5 border border-border px-3 py-1.5 text-xs">
              {va.name}
              <button type="button" onClick={() => handleRemove(va.id)} aria-label={`${va.name} kaldır`}>
                <X className="h-3 w-3" strokeWidth={1.5} />
              </button>
            </span>
          ))}
        </div>
      )}
      <form onSubmit={handleAdd} className="flex items-end gap-3">
        <div className="flex-1 max-w-xs">
          <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">Eksen Adı</label>
          <Input required value={name} onChange={(e) => setName(e.target.value)} placeholder="Beden" />
        </div>
        <button type="submit" disabled={submitting} className="border border-foreground px-4 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
          Ekle
        </button>
      </form>
      {error && (
        <div className="mt-3">
          <FormMessage tone="error">{error}</FormMessage>
        </div>
      )}
    </AdminSection>
  )
}

export function VariantsSection({ product, onSaved }: { product: Product; onSaved: () => void }) {
  const [sku, setSku] = useState("")
  const [optionValues, setOptionValues] = useState<Record<string, string>>({})
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  if (product.productType !== "Variant") return null

  const canAdd = product.variantAttributes.length > 0

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      await catalogApi.addProductVariant(product.id, { sku, optionValues })
      setSku("")
      setOptionValues({})
      onSaved()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Varyant eklenemedi.")
    } finally {
      setSubmitting(false)
    }
  }

  async function handleToggleActive(variantId: string, current: boolean, currentSku: string, currentValues: Record<string, string>) {
    await catalogApi.updateProductVariant(product.id, variantId, { sku: currentSku, optionValues: currentValues, isActive: !current })
    onSaved()
  }

  async function handleRemove(variantId: string) {
    if (!window.confirm("Bu varyantı silmek istediğinize emin misiniz?")) return
    await catalogApi.removeProductVariant(product.id, variantId)
    onSaved()
  }

  return (
    <AdminSection title="Varyantlar">
      {!canAdd ? (
        <p className="text-sm text-muted-foreground">Varyant eklemeden önce en az bir varyant ekseni tanımlayın (yukarıda).</p>
      ) : (
        <>
          {product.variants.length > 0 && (
            <Table>
              <THead>
                <TR>
                  <TH>SKU</TH>
                  <TH>Seçenekler</TH>
                  <TH>Durum</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {product.variants.map((v) => {
                  const values = Object.fromEntries(v.optionValues.map((o) => [o.productVariantAttributeId, o.value]))
                  return (
                    <TR key={v.id}>
                      <TD className="font-mono text-xs">{v.sku}</TD>
                      <TD className="text-muted-foreground">
                        <span className="flex flex-wrap items-center gap-2">
                          {v.optionValues.map((o, i) =>
                            isColorAttributeName(o.attributeName) ? (
                              <span key={i} className="flex items-center gap-1.5">
                                <span className="h-3.5 w-3.5 border border-border" style={{ backgroundColor: o.value }} />
                                {o.attributeName}
                              </span>
                            ) : (
                              <span key={i}>
                                {o.attributeName}: {o.value}
                              </span>
                            ),
                          )}
                        </span>
                      </TD>
                      <TD>
                        <button
                          type="button"
                          onClick={() => handleToggleActive(v.id, v.isActive, v.sku, values)}
                          className={`text-xs font-medium uppercase tracking-wider ${v.isActive ? "text-emerald-600" : "text-muted-foreground"}`}
                        >
                          {v.isActive ? "Aktif" : "Pasif"}
                        </button>
                      </TD>
                      <TD className="text-right">
                        <button type="button" onClick={() => handleRemove(v.id)} aria-label="Kaldır" className="text-muted-foreground hover:text-foreground">
                          <X className="h-4 w-4" strokeWidth={1.5} />
                        </button>
                      </TD>
                    </TR>
                  )
                })}
              </TBody>
            </Table>
          )}

          <form onSubmit={handleAdd} className="mt-4 space-y-3">
            <div className="max-w-xs">
              <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">SKU</label>
              <Input required value={sku} onChange={(e) => setSku(e.target.value)} />
            </div>
            <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
              {product.variantAttributes.map((va) => {
                const isColor = isColorAttributeName(va.name)
                return (
                  <div key={va.id}>
                    <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">{va.name}</label>
                    {isColor ? (
                      <div className="flex items-center gap-2">
                        <input
                          type="color"
                          required
                          value={toHexColorOrDefault(optionValues[va.id] ?? "")}
                          onChange={(e) => setOptionValues((prev) => ({ ...prev, [va.id]: e.target.value }))}
                          className="h-11 w-14 shrink-0 cursor-pointer border border-border bg-background p-1"
                        />
                        <span className="font-mono text-xs text-muted-foreground">{toHexColorOrDefault(optionValues[va.id] ?? "")}</span>
                      </div>
                    ) : (
                      <Input
                        required
                        value={optionValues[va.id] ?? ""}
                        onChange={(e) => setOptionValues((prev) => ({ ...prev, [va.id]: e.target.value }))}
                      />
                    )}
                  </div>
                )
              })}
            </div>
            <button type="submit" disabled={submitting} className="border border-foreground px-4 py-2.5 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
              Varyant Ekle
            </button>
          </form>
        </>
      )}
      {error && (
        <div className="mt-3">
          <FormMessage tone="error">{error}</FormMessage>
        </div>
      )}
    </AdminSection>
  )
}

export function ImagesSection({ product, onSaved }: { product: Product; onSaved: () => void }) {
  const [url, setUrl] = useState("")
  const [productVariantId, setProductVariantId] = useState("")
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      await catalogApi.addProductImage(product.id, {
        url,
        displayOrder: product.images.length,
        isPrimary: product.images.length === 0,
        productVariantId: productVariantId || null,
      })
      setUrl("")
      onSaved()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Görsel eklenemedi.")
    } finally {
      setSubmitting(false)
    }
  }

  async function handleRemove(imageId: string) {
    await catalogApi.removeProductImage(product.id, imageId)
    onSaved()
  }

  async function handleSetPrimary(imageId: string) {
    await catalogApi.setPrimaryProductImage(product.id, imageId)
    onSaved()
  }

  return (
    <AdminSection title="Görseller">
      {product.images.length > 0 && (
        <div className="mb-4 grid grid-cols-2 gap-3 sm:grid-cols-4">
          {product.images.map((img) => (
            <div key={img.id} className="relative aspect-square overflow-hidden border border-border">
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={img.url} alt="" className="h-full w-full object-cover" />
              <div className="absolute inset-x-0 bottom-0 flex items-center justify-between gap-1 bg-background/90 p-1.5">
                <button
                  type="button"
                  onClick={() => handleSetPrimary(img.id)}
                  className={`text-[9px] font-medium uppercase tracking-wider ${img.isPrimary ? "text-foreground" : "text-muted-foreground hover:text-foreground"}`}
                >
                  {img.isPrimary ? "Kapak" : "Kapak Yap"}
                </button>
                <button type="button" onClick={() => handleRemove(img.id)} aria-label="Kaldır">
                  <X className="h-3.5 w-3.5" strokeWidth={1.5} />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
      <form onSubmit={handleAdd} className="flex flex-wrap items-end gap-3">
        <div className="flex-1 min-w-[220px]">
          <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">Görsel URL</label>
          <Input required type="url" value={url} onChange={(e) => setUrl(e.target.value)} placeholder="https://…" />
        </div>
        {product.productType === "Variant" && product.variants.length > 0 && (
          <div className="min-w-[160px]">
            <label className="mb-1 block text-[10px] uppercase tracking-widest text-muted-foreground">Varyanta özel (opsiyonel)</label>
            <select
              value={productVariantId}
              onChange={(e) => setProductVariantId(e.target.value)}
              className="w-full border border-border bg-background px-3 py-3 text-sm outline-none focus:border-foreground"
            >
              <option value="">Genel görsel</option>
              {product.variants.map((v) => (
                <option key={v.id} value={v.id}>
                  {v.sku}
                </option>
              ))}
            </select>
          </div>
        )}
        <button type="submit" disabled={submitting} className="border border-foreground px-4 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
          Ekle
        </button>
      </form>
      {error && (
        <div className="mt-3">
          <FormMessage tone="error">{error}</FormMessage>
        </div>
      )}
    </AdminSection>
  )
}
