"use client"

import { useEffect, useState } from "react"
import * as shippingApi from "@/lib/api/shipping"
import type { ShippingCompany } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { formatPrice } from "@/lib/format"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { FormMessage } from "@/components/ui/form-message"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 20
const emptyForm = { name: "", fee: "" }

export function ShippingCompanyManager() {
  const [companies, setCompanies] = useState<ShippingCompany[]>([])
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  const [formOpen, setFormOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [form, setForm] = useState(emptyForm)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const result = await shippingApi.getShippingCompanies(pageNumber, PAGE_SIZE)
      setCompanies(result.items)
      setTotalPages(result.totalPages || 1)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber])

  function openNew() {
    setForm(emptyForm)
    setEditingId(null)
    setFormOpen(true)
  }

  function openEdit(c: ShippingCompany) {
    setForm({ name: c.name, fee: String(c.fee) })
    setEditingId(c.id)
    setFormOpen(true)
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      const input = { name: form.name, fee: Number(form.fee) }
      if (editingId) await shippingApi.updateShippingCompany(editingId, input)
      else await shippingApi.createShippingCompany(input)
      setFormOpen(false)
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  async function handleDeactivate(id: string) {
    if (!window.confirm("Bu kargo firmasını pasifleştirmek istediğinize emin misiniz?")) return
    await shippingApi.deactivateShippingCompany(id)
    await load()
  }

  return (
    <div>
      <AdminPageHeader
        title="Kargo Firmaları"
        action={
          !formOpen && (
            <button type="button" onClick={openNew} className="bg-foreground px-4 py-2 text-xs font-medium uppercase tracking-widest text-background hover:opacity-80">
              + Yeni Firma
            </button>
          )
        }
      />

      {formOpen && (
        <AdminSection title={editingId ? "Firmayı Düzenle" : "Yeni Firma"}>
          <form onSubmit={handleSubmit} className="flex max-w-lg flex-wrap items-end gap-4">
            <div className="flex-1 min-w-[200px]">
              <Label>Firma Adı</Label>
              <Input required value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
            </div>
            <div className="w-32">
              <Label>Ücret</Label>
              <Input required type="number" min="0" step="0.01" value={form.fee} onChange={(e) => setForm({ ...form, fee: e.target.value })} />
            </div>
            <button type="submit" disabled={saving} className="border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
              {saving ? "Kaydediliyor…" : "Kaydet"}
            </button>
            <button type="button" onClick={() => setFormOpen(false)} className="px-4 py-3 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
              Vazgeç
            </button>
          </form>
          {error && (
            <div className="mt-3">
              <FormMessage tone="error">{error}</FormMessage>
            </div>
          )}
        </AdminSection>
      )}

      <div className="mt-6">
        <AdminSection>
          {loading ? (
            <PageSpinner label="Kargo firmaları yükleniyor…" />
          ) : companies.length === 0 ? (
            <EmptyState title="Kargo firması bulunamadı" />
          ) : (
            <>
              <Table>
                <THead>
                  <TR>
                    <TH>Firma</TH>
                    <TH>Ücret</TH>
                    <TH>Durum</TH>
                    <TH></TH>
                  </TR>
                </THead>
                <TBody>
                  {companies.map((c) => (
                    <TR key={c.id}>
                      <TD className="font-medium">{c.name}</TD>
                      <TD className="tabular-nums">{formatPrice(c.fee)}</TD>
                      <TD>
                        <Badge tone={c.isActive ? "success" : "neutral"}>{c.isActive ? "Aktif" : "Pasif"}</Badge>
                      </TD>
                      <TD className="text-right">
                        <div className="flex justify-end gap-4 text-xs font-medium uppercase tracking-widest">
                          <button type="button" onClick={() => openEdit(c)} className="text-muted-foreground hover:text-foreground">
                            Düzenle
                          </button>
                          {c.isActive && (
                            <button type="button" onClick={() => handleDeactivate(c.id)} className="text-muted-foreground hover:text-foreground">
                              Pasifleştir
                            </button>
                          )}
                        </div>
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
    </div>
  )
}
