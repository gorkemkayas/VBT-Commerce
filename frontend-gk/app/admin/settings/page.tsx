"use client"

import { useEffect, useState } from "react"
import * as pricingApi from "@/lib/api/pricing"
import { ApiError } from "@/lib/api/client"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { FormMessage } from "@/components/ui/form-message"
import { PageSpinner } from "@/components/ui/spinner"

export default function AdminSettingsPage() {
  const [rate, setRate] = useState("")
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  useEffect(() => {
    pricingApi
      .getTaxRate()
      .then((r) => setRate(String(r)))
      .finally(() => setLoading(false))
  }, [])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      await pricingApi.updateTaxRate(Number(rate))
      setSuccess(true)
      setTimeout(() => setSuccess(false), 2000)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  return (
    <div>
      <AdminPageHeader title="Ayarlar" description="Mağaza geneli ayarlar." />
      <AdminSection title="Vergi Oranı (KDV)">
        {loading ? (
          <PageSpinner label="Yükleniyor…" />
        ) : (
          <form onSubmit={handleSubmit} className="max-w-xs space-y-4">
            <div>
              <Label>Oran (%)</Label>
              <Input type="number" min="0" max="100" step="0.01" value={rate} onChange={(e) => setRate(e.target.value)} />
            </div>
            {error && <FormMessage tone="error">{error}</FormMessage>}
            {success && <FormMessage tone="success">Kaydedildi.</FormMessage>}
            <button type="submit" disabled={saving} className="border border-foreground px-6 py-2.5 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background disabled:opacity-50">
              {saving ? "Kaydediliyor…" : "Kaydet"}
            </button>
          </form>
        )}
      </AdminSection>
    </div>
  )
}
