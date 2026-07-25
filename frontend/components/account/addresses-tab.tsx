"use client"

import { useEffect, useState } from "react"
import * as customersApi from "@/lib/api/customers"
import type { CustomerAddress } from "@/lib/api/types"
import { ApiError } from "@/lib/api/client"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { FormMessage } from "@/components/ui/form-message"
import { PageSpinner } from "@/components/ui/spinner"

const emptyForm = {
  label: "",
  recipientName: "",
  phoneNumber: "",
  country: "Türkiye",
  city: "",
  district: "",
  postalCode: "",
  addressLine1: "",
  addressLine2: "",
  isDefault: false,
  isShippingAddress: true,
  isBillingAddress: true,
}

export function AddressesTab() {
  const [addresses, setAddresses] = useState<CustomerAddress[]>([])
  const [loading, setLoading] = useState(true)
  const [formOpen, setFormOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [form, setForm] = useState(emptyForm)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const profile = await customersApi.getMyProfile()
      setAddresses(profile.addresses)
    } catch (err) {
      if (!(err instanceof ApiError && err.status === 404)) throw err
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  function openNew() {
    setForm(emptyForm)
    setEditingId(null)
    setFormOpen(true)
  }

  function openEdit(addr: CustomerAddress) {
    setForm({ ...addr, addressLine2: addr.addressLine2 ?? "" })
    setEditingId(addr.id)
    setFormOpen(true)
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      const input = { ...form, addressLine2: form.addressLine2 || null }
      if (editingId) await customersApi.updateMyAddress(editingId, input)
      else await customersApi.addMyAddress(input)
      setFormOpen(false)
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Adres kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete(id: string) {
    if (!window.confirm("Bu adresi silmek istediğinize emin misiniz?")) return
    await customersApi.removeMyAddress(id)
    await load()
  }

  async function handleSetDefault(id: string) {
    await customersApi.setDefaultMyAddress(id)
    await load()
  }

  if (loading) return <PageSpinner label="Adresler yükleniyor…" />

  return (
    <section aria-label="Adreslerim">
      {formOpen ? (
        <form onSubmit={handleSubmit} className="max-w-lg space-y-4 border border-border p-6">
          <h3 className="text-xs font-medium uppercase tracking-widest">{editingId ? "Adresi Düzenle" : "Yeni Adres"}</h3>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field label="Etiket (Ev, İş vb.)">
              <Input required value={form.label} onChange={(e) => setForm({ ...form, label: e.target.value })} />
            </Field>
            <Field label="Alıcı Adı Soyadı">
              <Input required value={form.recipientName} onChange={(e) => setForm({ ...form, recipientName: e.target.value })} />
            </Field>
            <Field label="Telefon">
              <Input required value={form.phoneNumber} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} />
            </Field>
            <Field label="Ülke">
              <Input required value={form.country} onChange={(e) => setForm({ ...form, country: e.target.value })} />
            </Field>
            <Field label="Şehir">
              <Input required value={form.city} onChange={(e) => setForm({ ...form, city: e.target.value })} />
            </Field>
            <Field label="İlçe">
              <Input required value={form.district} onChange={(e) => setForm({ ...form, district: e.target.value })} />
            </Field>
            <Field label="Posta Kodu">
              <Input required value={form.postalCode} onChange={(e) => setForm({ ...form, postalCode: e.target.value })} />
            </Field>
            <Field label="Adres Satırı 1" className="sm:col-span-2">
              <Input required value={form.addressLine1} onChange={(e) => setForm({ ...form, addressLine1: e.target.value })} />
            </Field>
            <Field label="Adres Satırı 2 (opsiyonel)" className="sm:col-span-2">
              <Input value={form.addressLine2} onChange={(e) => setForm({ ...form, addressLine2: e.target.value })} />
            </Field>
          </div>
          <div className="flex flex-wrap gap-4">
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" checked={form.isDefault} onChange={(e) => setForm({ ...form, isDefault: e.target.checked })} className="accent-foreground" />
              Varsayılan adres olarak ayarla
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={form.isShippingAddress}
                onChange={(e) => setForm({ ...form, isShippingAddress: e.target.checked })}
                className="accent-foreground"
              />
              Teslimat adresi olarak kullanılabilsin
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={form.isBillingAddress}
                onChange={(e) => setForm({ ...form, isBillingAddress: e.target.checked })}
                className="accent-foreground"
              />
              Fatura adresi olarak kullanılabilsin
            </label>
          </div>
          {!form.isShippingAddress && !form.isBillingAddress && (
            <FormMessage tone="error">Adres en az teslimat ya da fatura adresi olarak işaretlenmeli.</FormMessage>
          )}
          {error && <FormMessage tone="error">{error}</FormMessage>}
          <div className="flex gap-3">
            <button
              type="submit"
              disabled={saving || (!form.isShippingAddress && !form.isBillingAddress)}
              className="border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50"
            >

              {saving ? "Kaydediliyor…" : "Kaydet"}
            </button>
            <button type="button" onClick={() => setFormOpen(false)} className="px-6 py-3 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
              Vazgeç
            </button>
          </div>
        </form>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2">
          {addresses.map((address) => (
            <article key={address.id} className="border border-border p-5">
              <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
                <span className="text-sm font-semibold">{address.label}</span>
                <div className="flex flex-wrap gap-1.5">
                  {address.isDefault && (
                    <span className="border border-foreground px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider">Varsayılan</span>
                  )}
                  {address.isShippingAddress && (
                    <span className="border border-border px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider text-muted-foreground">Teslimat</span>
                  )}
                  {address.isBillingAddress && (
                    <span className="border border-border px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider text-muted-foreground">Fatura</span>
                  )}
                </div>
              </div>
              <p className="text-sm font-medium">{address.recipientName}</p>
              <p className="mt-1 text-sm text-muted-foreground">
                {address.addressLine1}
                {address.addressLine2 ? `, ${address.addressLine2}` : ""}
              </p>
              <p className="text-sm text-muted-foreground">
                {address.district}/{address.city}, {address.postalCode}
              </p>
              <p className="mt-2 text-sm text-muted-foreground">{address.phoneNumber}</p>
              <div className="mt-4 flex flex-wrap gap-4 text-xs font-medium uppercase tracking-widest">
                <button type="button" onClick={() => openEdit(address)} className="text-muted-foreground hover:text-foreground">
                  Düzenle
                </button>
                {!address.isDefault && (
                  <button type="button" onClick={() => handleSetDefault(address.id)} className="text-muted-foreground hover:text-foreground">
                    Varsayılan Yap
                  </button>
                )}
                <button type="button" onClick={() => handleDelete(address.id)} className="text-muted-foreground hover:text-foreground">
                  Sil
                </button>
              </div>
            </article>
          ))}
          <button
            type="button"
            onClick={openNew}
            className="flex min-h-[160px] items-center justify-center border border-dashed border-border text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:border-foreground hover:text-foreground"
          >
            + Yeni Adres Ekle
          </button>
        </div>
      )}
    </section>
  )
}

function Field({ label, children, className = "" }: { label: string; children: React.ReactNode; className?: string }) {
  return (
    <div className={className}>
      <Label>{label}</Label>
      {children}
    </div>
  )
}
