"use client"

import { useEffect, useState } from "react"
import Image from "next/image"
import Link from "next/link"
import { Plus } from "lucide-react"
import * as catalogApi from "@/lib/api/catalog"
import type { CategoryTree, ProductListItem } from "@/lib/api/types"
import { findCategoryName } from "@/lib/store-catalog"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Input } from "@/components/ui/input"
import { Select } from "@/components/ui/select"
import { Badge } from "@/components/ui/badge"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20

export function ProductList() {
  const [products, setProducts] = useState<ProductListItem[]>([])
  const [categories, setCategories] = useState<CategoryTree[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [searchTerm, setSearchTerm] = useState("")
  const [categoryId, setCategoryId] = useState("")
  const [isActiveFilter, setIsActiveFilter] = useState("")
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    catalogApi.getCategoryTree(true).then(setCategories)
  }, [])

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    const t = setTimeout(async () => {
      try {
        const result = await catalogApi.getProducts({
          pageNumber,
          pageSize: PAGE_SIZE,
          searchTerm: searchTerm || undefined,
          categoryId: categoryId || undefined,
          isActive: isActiveFilter === "" ? undefined : isActiveFilter === "true",
        })
        if (!cancelled) {
          setProducts(result.items)
          setTotalPages(result.totalPages || 1)
        }
      } finally {
        if (!cancelled) setLoading(false)
      }
    }, 250)
    return () => {
      cancelled = true
      clearTimeout(t)
    }
  }, [pageNumber, searchTerm, categoryId, isActiveFilter])

  return (
    <div>
      <AdminPageHeader
        title="Ürünler"
        description="Katalogdaki tüm ürünler."
        action={
          <Link href="/admin/products/new" className="flex items-center gap-1.5 bg-foreground px-4 py-2 text-xs font-medium uppercase tracking-widest text-background hover:opacity-80">
            <Plus className="h-3.5 w-3.5" strokeWidth={2} />
            Yeni Ürün
          </Link>
        }
      />

      <AdminSection>
        <div className="mb-4 flex flex-wrap gap-3">
          <Input
            value={searchTerm}
            onChange={(e) => {
              setPageNumber(1)
              setSearchTerm(e.target.value)
            }}
            placeholder="Ürün ara…"
            className="max-w-xs"
          />
          <Select
            value={categoryId}
            onChange={(e) => {
              setPageNumber(1)
              setCategoryId(e.target.value)
            }}
            className="max-w-xs"
          >
            <option value="">Tüm kategoriler</option>
            {flatten(categories).map(({ node, depth }) => (
              <option key={node.id} value={node.id}>
                {"—".repeat(depth)} {node.name}
              </option>
            ))}
          </Select>
          <Select
            value={isActiveFilter}
            onChange={(e) => {
              setPageNumber(1)
              setIsActiveFilter(e.target.value)
            }}
            className="max-w-[160px]"
          >
            <option value="">Tüm durumlar</option>
            <option value="true">Aktif</option>
            <option value="false">Pasif</option>
          </Select>
        </div>

        {loading ? (
          <PageSpinner label="Ürünler yükleniyor…" />
        ) : products.length === 0 ? (
          <EmptyState title="Ürün bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH></TH>
                  <TH>Ürün</TH>
                  <TH>Kategori</TH>
                  <TH>Tip</TH>
                  <TH>Durum</TH>
                  <TH></TH>
                </TR>
              </THead>
              <TBody>
                {products.map((p) => (
                  <TR key={p.id}>
                    <TD className="w-14">
                      <div className="relative h-12 w-10 overflow-hidden bg-secondary">
                        <Image src={p.primaryImageUrl || "/placeholder.svg"} alt="" fill sizes="40px" className="object-cover" />
                      </div>
                    </TD>
                    <TD>
                      <p className="font-medium">{p.name}</p>
                      <p className="text-xs text-muted-foreground">{p.slug}</p>
                    </TD>
                    <TD className="text-muted-foreground">{findCategoryName(categories, p.categoryId)}</TD>
                    <TD className="text-muted-foreground">{p.productType === "Simple" ? "Basit" : "Varyantlı"}</TD>
                    <TD>
                      <Badge tone={p.isActive ? "success" : "neutral"}>{p.isActive ? "Aktif" : "Pasif"}</Badge>
                    </TD>
                    <TD className="text-right">
                      <Link href={`/admin/products/${p.id}`} className="text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
                        Düzenle
                      </Link>
                    </TD>
                  </TR>
                ))}
              </TBody>
            </Table>
            <div className="mt-4">
              <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
            </div>
          </>
        )}
      </AdminSection>
    </div>
  )
}

function flatten(nodes: CategoryTree[], depth = 0): { node: CategoryTree; depth: number }[] {
  return nodes.flatMap((node) => [{ node, depth }, ...flatten(node.children, depth + 1)])
}
