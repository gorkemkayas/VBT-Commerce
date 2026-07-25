"use client"

import { Fragment, useEffect, useMemo, useState } from "react"
import Image from "next/image"
import { ChevronRight } from "lucide-react"
import * as inventoryApi from "@/lib/api/inventory"
import * as catalogApi from "@/lib/api/catalog"
import type { Product, ProductListItem, SellableItemType, StockItem } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { buildCatalogIndex, variantLabel, type CatalogEntry } from "@/lib/catalog-index"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { ProductPicker } from "@/components/admin/inventory/product-picker"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { Badge } from "@/components/ui/badge"
import { FormMessage } from "@/components/ui/form-message"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20

function StockRowActions({
  item,
  onAdjust,
}: {
  item: StockItem
  onAdjust: (stockItemId: string, direction: "increase" | "decrease") => void
}) {
  return (
    <div className="flex justify-end gap-3 text-xs font-medium uppercase tracking-widest">
      <button type="button" onClick={() => onAdjust(item.id, "increase")} className="text-muted-foreground hover:text-foreground">
        Artır
      </button>
      <button type="button" onClick={() => onAdjust(item.id, "decrease")} className="text-muted-foreground hover:text-foreground">
        Azalt
      </button>
    </div>
  )
}

export function StockItemList() {
  const [items, setItems] = useState<StockItem[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [typeFilter, setTypeFilter] = useState<SellableItemType | "">("")
  const [loading, setLoading] = useState(true)
  const [catalogIndex, setCatalogIndex] = useState<Map<string, CatalogEntry> | null>(null)

  const [selectedProduct, setSelectedProduct] = useState<ProductListItem | null>(null)
  const [productDetail, setProductDetail] = useState<Product | null>(null)
  const [loadingVariants, setLoadingVariants] = useState(false)
  const [selectedVariantId, setSelectedVariantId] = useState("")
  const [qty, setQty] = useState("1")
  const [createError, setCreateError] = useState<string | null>(null)
  const [creating, setCreating] = useState(false)

  async function load() {
    setLoading(true)
    try {
      const result = await inventoryApi.getStockItems({ pageNumber, pageSize: PAGE_SIZE, sellableItemType: typeFilter || undefined })
      setItems(result.items)
      setTotalPages(result.totalPages || 1)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber, typeFilter])

  useEffect(() => {
    buildCatalogIndex()
      .then(setCatalogIndex)
      .catch(() => setCatalogIndex(new Map()))
  }, [])

  const groups = useMemo(() => {
    const map = new Map<string, { key: string; productName: string; productImage: string | null; rows: StockItem[] }>()
    for (const item of items) {
      const entry = catalogIndex?.get(item.sellableItemId) ?? null
      const key = entry?.productId ?? item.sellableItemId
      if (!map.has(key)) {
        map.set(key, {
          key,
          productName: entry?.productName ?? "Bilinmeyen ürün",
          productImage: entry?.productImage ?? null,
          rows: [],
        })
      }
      map.get(key)!.rows.push(item)
    }
    return Array.from(map.values())
  }, [items, catalogIndex])

  const [expandedGroups, setExpandedGroups] = useState<Set<string>>(new Set())

  function toggleGroup(key: string) {
    setExpandedGroups((prev) => {
      const next = new Set(prev)
      if (next.has(key)) next.delete(key)
      else next.add(key)
      return next
    })
  }

  useEffect(() => {
    if (!selectedProduct || selectedProduct.productType !== "Variant") {
      setProductDetail(null)
      setSelectedVariantId("")
      return
    }
    let cancelled = false
    setLoadingVariants(true)
    catalogApi
      .getProductById(selectedProduct.id)
      .then((product) => {
        if (cancelled) return
        setProductDetail(product)
        setSelectedVariantId(product.variants.find((v) => v.isActive)?.id ?? "")
      })
      .finally(() => {
        if (!cancelled) setLoadingVariants(false)
      })
    return () => {
      cancelled = true
    }
  }, [selectedProduct])

  function resetForm() {
    setSelectedProduct(null)
    setProductDetail(null)
    setSelectedVariantId("")
    setQty("1")
  }

  const sellableItemId = selectedProduct
    ? selectedProduct.productType === "Variant"
      ? selectedVariantId
      : selectedProduct.id
    : ""
  const sellableItemType: SellableItemType = selectedProduct?.productType === "Variant" ? "Variant" : "Product"

  const variantPreviewImage =
    selectedVariantId && productDetail
      ? (productDetail.images.find((img) => img.productVariantId === selectedVariantId) ??
          productDetail.images.find((img) => !img.productVariantId))?.url ?? null
      : null

  async function handleAddStock(e: React.FormEvent) {
    e.preventDefault()
    setCreateError(null)
    if (!sellableItemId) {
      setCreateError("Lütfen bir ürün seçin.")
      return
    }
    const quantity = Number(qty)
    if (!Number.isFinite(quantity) || quantity <= 0) {
      setCreateError("Geçerli bir miktar girin.")
      return
    }
    setCreating(true)
    try {
      let existing: StockItem | null = null
      try {
        existing = await inventoryApi.getStockItemBySellableItem(sellableItemId, sellableItemType)
      } catch (err) {
        if (!(err instanceof ApiError && err.status === 404)) throw err
      }
      if (existing) {
        await inventoryApi.increaseStock(existing.id, quantity)
      } else {
        await inventoryApi.createStockItem({ sellableItemId, sellableItemType, initialQuantity: quantity })
      }
      resetForm()
      await load()
    } catch (err) {
      setCreateError(err instanceof ApiError ? err.message : "Stok eklenemedi.")
    } finally {
      setCreating(false)
    }
  }

  async function adjust(stockItemId: string, direction: "increase" | "decrease") {
    const amount = window.prompt(direction === "increase" ? "Eklenecek miktar:" : "Düşülecek miktar:", "1")
    if (!amount) return
    const quantity = Number(amount)
    if (!Number.isFinite(quantity) || quantity <= 0) return
    if (direction === "increase") await inventoryApi.increaseStock(stockItemId, quantity)
    else await inventoryApi.decreaseStock(stockItemId, quantity)
    await load()
  }

  return (
    <div>
      <AdminPageHeader title="Stok Kalemleri" description="Ürün ve varyantların stok seviyeleri." />

      <AdminSection title="Stok Ekle">
        <form onSubmit={handleAddStock} className="flex flex-wrap items-end gap-3">
          <div className="min-w-[280px] flex-1">
            <Label>Ürün</Label>
            <ProductPicker
              selectedProduct={selectedProduct}
              previewImageUrl={variantPreviewImage}
              onSelect={setSelectedProduct}
              onClear={resetForm}
            />
          </div>

          {selectedProduct?.productType === "Variant" && (
            <div className="min-w-[200px]">
              <Label>Varyant</Label>
              {loadingVariants ? (
                <p className="py-3 text-sm text-muted-foreground">Yükleniyor…</p>
              ) : (
                <Select value={selectedVariantId} onChange={(e) => setSelectedVariantId(e.target.value)}>
                  <option value="">Varyant seçin</option>
                  {productDetail?.variants.map((v) => (
                    <option key={v.id} value={v.id}>
                      {variantLabel(v)}
                      {!v.isActive ? " (pasif)" : ""}
                    </option>
                  ))}
                </Select>
              )}
            </div>
          )}

          <div className="w-28">
            <Label>Miktar</Label>
            <Input type="number" min="1" value={qty} onChange={(e) => setQty(e.target.value)} />
          </div>

          <button
            type="submit"
            disabled={creating || !sellableItemId}
            className="border border-foreground px-4 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50"
          >
            Stok Ekle
          </button>
        </form>
        {createError && (
          <div className="mt-3">
            <FormMessage tone="error">{createError}</FormMessage>
          </div>
        )}
      </AdminSection>

      <div className="mt-6">
        <AdminSection>
          <div className="mb-4">
            <Select value={typeFilter} onChange={(e) => setTypeFilter(e.target.value as SellableItemType | "")} className="max-w-[180px]">
              <option value="">Tüm tipler</option>
              <option value="Product">Ürün</option>
              <option value="Variant">Varyant</option>
            </Select>
          </div>
          {loading ? (
            <PageSpinner label="Stok listesi yükleniyor…" />
          ) : items.length === 0 ? (
            <EmptyState title="Stok kalemi bulunamadı" />
          ) : (
            <>
              <Table>
                <THead>
                  <TR>
                    <TH>Ürün</TH>
                    <TH>Eldeki</TH>
                    <TH>Satılabilir</TH>
                    <TH></TH>
                  </TR>
                </THead>
                <TBody>
                  {groups.map((group) => {
                    const isSingleSimpleProduct = group.rows.length === 1 && group.rows[0].sellableItemType === "Product"

                    if (isSingleSimpleProduct) {
                      const item = group.rows[0]
                      return (
                        <TR key={item.id}>
                          <TD>
                            <div className="flex items-center gap-3">
                              <div className="relative h-9 w-9 shrink-0 overflow-hidden bg-secondary">
                                <Image src={group.productImage || "/placeholder.svg"} alt="" fill sizes="36px" className="object-cover" />
                              </div>
                              <div className="min-w-0">
                                <p className="truncate text-sm">{group.productName}</p>
                                <p className="truncate font-mono text-[10px] text-muted-foreground">{item.sellableItemId}</p>
                              </div>
                            </div>
                          </TD>
                          <TD className="tabular-nums">{item.quantityOnHand}</TD>
                          <TD className="tabular-nums">{item.availableQuantity}</TD>
                          <TD className="text-right">
                            <StockRowActions item={item} onAdjust={adjust} />
                          </TD>
                        </TR>
                      )
                    }

                    const isOpen = expandedGroups.has(group.key)

                    return (
                      <Fragment key={group.key}>
                        <TR className="cursor-pointer bg-secondary/30 hover:bg-secondary/50" onClick={() => toggleGroup(group.key)}>
                          <TD colSpan={4}>
                            <div className="flex items-center gap-3">
                              <ChevronRight
                                className={`h-3.5 w-3.5 shrink-0 text-muted-foreground transition-transform ${isOpen ? "rotate-90" : ""}`}
                                strokeWidth={1.5}
                              />
                              <div className="relative h-9 w-9 shrink-0 overflow-hidden bg-secondary">
                                <Image src={group.productImage || "/placeholder.svg"} alt="" fill sizes="36px" className="object-cover" />
                              </div>
                              <p className="text-sm font-medium">{group.productName}</p>
                              <Badge tone="neutral">{group.rows.length} varyant</Badge>
                            </div>
                          </TD>
                        </TR>
                        {isOpen &&
                          group.rows.map((item) => {
                            const entry = catalogIndex?.get(item.sellableItemId) ?? null
                            return (
                              <TR key={item.id}>
                                <TD>
                                  <div className="flex items-center gap-3 pl-8">
                                    <div className="relative h-8 w-8 shrink-0 overflow-hidden bg-secondary">
                                      <Image
                                        src={entry?.variantImage || group.productImage || "/placeholder.svg"}
                                        alt=""
                                        fill
                                        sizes="32px"
                                        className="object-cover"
                                      />
                                    </div>
                                    <div className="min-w-0">
                                      <p className="truncate text-sm text-muted-foreground">
                                        {entry?.variant ? variantLabel(entry.variant) : item.sellableItemId}
                                      </p>
                                    </div>
                                  </div>
                                </TD>
                                <TD className="tabular-nums">{item.quantityOnHand}</TD>
                                <TD className="tabular-nums">{item.availableQuantity}</TD>
                                <TD className="text-right">
                                  <StockRowActions item={item} onAdjust={adjust} />
                                </TD>
                              </TR>
                            )
                          })}
                      </Fragment>
                    )
                  })}
                </TBody>
              </Table>
              <div className="mt-4">
                <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
              </div>
            </>
          )}
        </AdminSection>
      </div>
    </div>
  )
}
