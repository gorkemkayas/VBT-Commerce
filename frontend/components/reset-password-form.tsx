"use client"

import { useState } from "react"
import Link from "next/link"
import { useRouter, useSearchParams } from "next/navigation"
import { resetPassword } from "@/lib/api/auth"
import { extractErrorMessage } from "@/lib/auth-context"
import { FormMessage } from "@/components/ui/form-message"

export function ResetPasswordForm() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const token = searchParams.get("token") ?? ""

  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    if (password !== confirmPassword) {
      setError("Şifreler eşleşmiyor.")
      return
    }
    setSubmitting(true)
    try {
      await resetPassword(token, password)
      setSuccess(true)
      setTimeout(() => router.push("/login"), 2000)
    } catch (err) {
      setError(extractErrorMessage(err, "Şifre sıfırlanamadı. Bağlantının süresi dolmuş olabilir."))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="w-full max-w-sm">
      <Link href="/" className="mb-10 inline-block font-brand text-2xl font-normal tracking-[0.15em]">
        Trendora
      </Link>
      <p className="text-xs uppercase tracking-widest text-muted-foreground">Hesap kurtarma</p>
      <h1 className="mt-2 font-serif text-4xl font-medium tracking-tight">Yeni Şifre Belirle</h1>

      {!token && <FormMessage tone="error">Geçersiz bağlantı: sıfırlama kodu eksik.</FormMessage>}

      {success ? (
        <div className="mt-8">
          <FormMessage tone="success">Şifreniz güncellendi. Giriş sayfasına yönlendiriliyorsunuz…</FormMessage>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="mt-8 space-y-5">
          <div>
            <label className="mb-2 block text-xs uppercase tracking-widest text-muted-foreground">Yeni Şifre</label>
            <input
              type="password"
              required
              minLength={6}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              className="w-full border border-border bg-background px-4 py-3 text-sm outline-none transition-colors focus:border-foreground"
            />
          </div>
          <div>
            <label className="mb-2 block text-xs uppercase tracking-widest text-muted-foreground">Yeni Şifre (Tekrar)</label>
            <input
              type="password"
              required
              minLength={6}
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="••••••••"
              className="w-full border border-border bg-background px-4 py-3 text-sm outline-none transition-colors focus:border-foreground"
            />
          </div>
          {error && <FormMessage tone="error">{error}</FormMessage>}
          <button
            type="submit"
            disabled={submitting || !token}
            className="w-full bg-foreground py-3 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-90 disabled:opacity-60"
          >
            {submitting ? "Kaydediliyor…" : "Şifreyi Güncelle"}
          </button>
        </form>
      )}
    </div>
  )
}
