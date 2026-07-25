"use client"

import { useCallback, useEffect, useState } from "react"
import * as catalogApi from "@/lib/api/catalog"
import type { CategoryTree } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { FormMessage } from "@/components/ui/form-message"
import { Badge } from "@/components/ui/badge"
import { PageSpinner } from "@/components/ui/spinner"

const emptyForm = { name: "", slug: "", description: "", imageUrl: "", parentCategoryId: "", displayOrder: 0 }

export function CategoryManager() {
  const [tree, setTree] = useState<CategoryTree[]>([])
  const [loading, setLoading] = useState(true)
  const [formOpen, setFormOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [form, setForm] = useState(emptyForm)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      setTree(await catalogApi.getCategoryTree(true))
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    load()
  }, [load])

  const flat = flatten(tree)

  function openNew() {
    setForm({ ...emptyForm, displayOrder: flat.length })
    setEditingId(null)
    setFormOpen(true)
  }

  function openEdit(node: CategoryTree, parentId: string | null) {
    setForm({
      name: node.name,
      slug: node.slug,
      description: node.description ?? "",
      imageUrl: node.imageUrl ?? "",
      parentCategoryId: parentId ?? "",
      displayOrder: node.displayOrder,
    })
    setEditingId(node.id)
    setFormOpen(true)
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      const input = {
        name: form.name,
        slug: form.slug,
        description: form.description || null,
        imageUrl: form.imageUrl || null,
        parentCategoryId: form.parentCategoryId || null,
        displayOrder: Number(form.displayOrder),
      }
      if (editingId) await catalogApi.updateCategory(editingId, input)
      else await catalogApi.createCategory(input)
      setFormOpen(false)
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete(id: string, name: string) {
    if (!window.confirm(`"${name}" kategorisini silmek istediğinize emin misiniz?`)) return
    try {
      await catalogApi.deleteCategory(id)
      await load()
    } catch (err) {
      window.alert(err instanceof ApiError ? err.message : "Kategori silinemedi.")
    }
  }

  return (
    <div>
      <AdminPageHeader
        title="Kategoriler"
        description="Ürün kategori ağacı."
        action={
          !formOpen && (
            <button type="button" onClick={openNew} className="bg-foreground px-4 py-2 text-xs font-medium uppercase tracking-widest text-background hover:opacity-80">
              + Yeni Kategori
            </button>
          )
        }
      />

      {formOpen && (
        <AdminSection title={editingId ? "Kategoriyi Düzenle" : "Yeni Kategori"}>
          <form onSubmit={handleSubmit} className="grid max-w-2xl grid-cols-1 gap-4 sm:grid-cols-2">
            <div>
              <Label>Ad</Label>
              <Input required value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
            </div>
            <div>
              <Label>Slug</Label>
              <Input required value={form.slug} onChange={(e) => setForm({ ...form, slug: e.target.value })} />
            </div>
            <div className="sm:col-span-2">
              <Label>Açıklama</Label>
              <Textarea rows={2} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            </div>
            <div className="sm:col-span-2">
              <Label>Görsel URL</Label>
              <Input
                type="url"
                placeholder="https://…"
                value={form.imageUrl}
                onChange={(e) => setForm({ ...form, imageUrl: e.target.value })}
              />
              {form.imageUrl && (
                // eslint-disable-next-line @next/next/no-img-element
                <img src={form.imageUrl} alt="" className="mt-2 h-24 w-24 border border-border object-cover" />
              )}
            </div>
            <div>
              <Label>Üst Kategori</Label>
              <Select value={form.parentCategoryId} onChange={(e) => setForm({ ...form, parentCategoryId: e.target.value })}>
                <option value="">Yok (kök kategori)</option>
                {flat
                  .filter((f) => f.node.id !== editingId)
                  .map(({ node, depth }) => (
                    <option key={node.id} value={node.id}>
                      {"—".repeat(depth)} {node.name}
                    </option>
                  ))}
              </Select>
            </div>
            <div>
              <Label>Sıra</Label>
              <Input type="number" value={form.displayOrder} onChange={(e) => setForm({ ...form, displayOrder: Number(e.target.value) })} />
            </div>
            {error && (
              <div className="sm:col-span-2">
                <FormMessage tone="error">{error}</FormMessage>
              </div>
            )}
            <div className="flex gap-3 sm:col-span-2">
              <button type="submit" disabled={saving} className="border border-foreground px-6 py-2.5 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
                {saving ? "Kaydediliyor…" : "Kaydet"}
              </button>
              <button type="button" onClick={() => setFormOpen(false)} className="px-6 py-2.5 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
                Vazgeç
              </button>
            </div>
          </form>
        </AdminSection>
      )}

      <div className="mt-6">
        <AdminSection>
          {loading ? (
            <PageSpinner label="Kategoriler yükleniyor…" />
          ) : flat.length === 0 ? (
            <p className="text-sm text-muted-foreground">Henüz kategori yok.</p>
          ) : (
            <ul className="divide-y divide-border">
              {flat.map(({ node, depth, parentId }) => (
                <li key={node.id} className="flex items-center justify-between gap-3 py-3" style={{ paddingLeft: `${depth * 1.5}rem` }}>
                  <div className="flex items-center gap-3">
                    <span className="text-sm font-medium">{node.name}</span>
                    <span className="text-xs text-muted-foreground">/{node.slug}</span>
                    <Badge tone={node.isActive ? "success" : "neutral"}>{node.isActive ? "Aktif" : "Pasif"}</Badge>
                  </div>
                  <div className="flex gap-4 text-xs font-medium uppercase tracking-widest">
                    <button type="button" onClick={() => openEdit(node, parentId)} className="text-muted-foreground hover:text-foreground">
                      Düzenle
                    </button>
                    <button type="button" onClick={() => handleDelete(node.id, node.name)} className="text-muted-foreground hover:text-foreground">
                      Sil
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </AdminSection>
      </div>
    </div>
  )
}

function flatten(nodes: CategoryTree[], depth = 0, parentId: string | null = null): { node: CategoryTree; depth: number; parentId: string | null }[] {
  return nodes.flatMap((node) => [{ node, depth, parentId }, ...flatten(node.children, depth + 1, node.id)])
}
