"use client"

import { useEffect, useState } from "react"
import * as customersApi from "@/lib/api/customers"
import { ApiError } from "@/lib/api/client"
import { useAuth } from "@/lib/auth-context"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { FormMessage } from "@/components/ui/form-message"
import { PageSpinner } from "@/components/ui/spinner"

export function ProfileTab() {
  const { user } = useAuth()
  const [loading, setLoading] = useState(true)
  const [exists, setExists] = useState(false)
  const [editing, setEditing] = useState(false)
  const [phoneNumber, setPhoneNumber] = useState("")
  const [dateOfBirth, setDateOfBirth] = useState("")
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  useEffect(() => {
    ;(async () => {
      try {
        const profile = await customersApi.getMyProfile()
        setExists(true)
        setPhoneNumber(profile.phoneNumber ?? "")
        setDateOfBirth(profile.dateOfBirth ?? "")
      } catch (err) {
        if (err instanceof ApiError && err.status === 404) {
          setExists(false)
          setEditing(true)
        }
      } finally {
        setLoading(false)
      }
    })()
  }, [])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      const input = { phoneNumber: phoneNumber || null, dateOfBirth: dateOfBirth || null }
      if (exists) await customersApi.updateMyProfile(input)
      else await customersApi.createMyProfile(input)
      setExists(true)
      setEditing(false)
      setSuccess(true)
      setTimeout(() => setSuccess(false), 2000)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kaydedilemedi.")
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <PageSpinner label="Profil yükleniyor…" />

  if (!editing) {
    return (
      <section className="max-w-lg" aria-label="Kişisel bilgiler">
        <dl className="divide-y divide-border border-y border-border">
          <Row label="E-posta" value={user?.email ?? "—"} />
          <Row label="Telefon" value={phoneNumber || "—"} />
          <Row label="Doğum Tarihi" value={dateOfBirth || "—"} />
        </dl>
        {success && (
          <div className="mt-4">
            <FormMessage tone="success">Bilgileriniz güncellendi.</FormMessage>
          </div>
        )}
        <button
          type="button"
          onClick={() => setEditing(true)}
          className="mt-6 border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest transition-colors hover:bg-foreground hover:text-background"
        >
          Bilgileri Düzenle
        </button>
      </section>
    )
  }

  return (
    <form onSubmit={handleSubmit} className="max-w-lg space-y-5">
      <div>
        <Label>Telefon</Label>
        <Input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} placeholder="5551234567" />
      </div>
      <div>
        <Label>Doğum Tarihi</Label>
        <Input type="date" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} />
      </div>
      {error && <FormMessage tone="error">{error}</FormMessage>}
      <div className="flex gap-3">
        <button
          type="submit"
          disabled={saving}
          className="border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest transition-colors hover:bg-foreground hover:text-background disabled:opacity-50"
        >
          {saving ? "Kaydediliyor…" : "Kaydet"}
        </button>
        {exists && (
          <button type="button" onClick={() => setEditing(false)} className="px-6 py-3 text-xs font-medium uppercase tracking-widest text-muted-foreground hover:text-foreground">
            Vazgeç
          </button>
        )}
      </div>
    </form>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4 py-4">
      <dt className="text-xs uppercase tracking-widest text-muted-foreground">{label}</dt>
      <dd className="text-sm font-medium">{value}</dd>
    </div>
  )
}
