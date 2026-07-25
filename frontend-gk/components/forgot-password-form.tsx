"use client"

import { useState } from "react"
import Link from "next/link"
import { forgotPassword } from "@/lib/api/auth"
import { extractErrorMessage } from "@/lib/auth-context"
import { FormMessage } from "@/components/ui/form-message"

export function ForgotPasswordForm() {
  const [email, setEmail] = useState("")
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [sent, setSent] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      await forgotPassword(email)
      setSent(true)
    } catch (err) {
      setError(extractErrorMessage(err, "İşlem başarısız oldu."))
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
      <h1 className="mt-2 font-serif text-4xl font-medium tracking-tight">Şifremi Unuttum</h1>
      <p className="mt-3 text-sm text-muted-foreground">
        E-posta adresinizi girin, şifre sıfırlama bağlantısını gönderelim.
      </p>

      {sent ? (
        <div className="mt-8">
          <FormMessage tone="success">
            Eğer bu e-posta adresine kayıtlı bir hesap varsa, şifre sıfırlama bağlantısı gönderildi.
          </FormMessage>
          <Link href="/login" className="mt-6 inline-block text-sm font-medium underline underline-offset-4">
            Giriş sayfasına dön
          </Link>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="mt-8 space-y-5">
          <div>
            <label className="mb-2 block text-xs uppercase tracking-widest text-muted-foreground">E-posta</label>
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="ornek@mail.com"
              className="w-full border border-border bg-background px-4 py-3 text-sm outline-none transition-colors focus:border-foreground"
            />
          </div>
          {error && <FormMessage tone="error">{error}</FormMessage>}
          <button
            type="submit"
            disabled={submitting}
            className="w-full bg-foreground py-3 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-90 disabled:opacity-60"
          >
            {submitting ? "Gönderiliyor…" : "Sıfırlama Bağlantısı Gönder"}
          </button>
          <Link href="/login" className="block text-center text-sm text-muted-foreground underline underline-offset-4">
            Giriş sayfasına dön
          </Link>
        </form>
      )}
    </div>
  )
}
