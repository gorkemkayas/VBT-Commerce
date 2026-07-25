"use client"

import { useState } from "react"
import Link from "next/link"
import Image from "next/image"
import { useRouter, useSearchParams } from "next/navigation"
import { Eye, EyeOff } from "lucide-react"
import { useAuth, extractErrorMessage } from "@/lib/auth-context"
import { FormMessage } from "@/components/ui/form-message"

type Mode = "login" | "register"

export function AuthForm({ mode }: { mode: Mode }) {
  const router = useRouter()
  const searchParams = useSearchParams()
  const { login, register } = useAuth()
  const [showPassword, setShowPassword] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [firstName, setFirstName] = useState("")
  const [lastName, setLastName] = useState("")

  const isLogin = mode === "login"

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      if (isLogin) {
        await login(email, password)
      } else {
        await register({ email, password, firstName, lastName })
      }
      const redirectTo = searchParams.get("redirect") || "/account"
      router.push(redirectTo)
    } catch (err) {
      setError(extractErrorMessage(err, isLogin ? "Giriş yapılamadı. Bilgilerinizi kontrol edin." : "Kayıt oluşturulamadı."))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="grid min-h-[calc(100vh-var(--header-h,0px))] md:grid-cols-2">
      {/* Görsel taraf */}
      <div className="relative hidden md:block">
        <Image
          src="/editorial.png"
          alt="Trendora moda editoryel görsel"
          fill
          className="object-cover grayscale"
          sizes="50vw"
          priority
        />
        <div className="absolute inset-0 flex flex-col justify-between p-10">
          <Link href="/" className="font-brand text-2xl font-normal tracking-[0.15em] text-background">
            Trendora
          </Link>
          <p className="max-w-xs text-sm leading-relaxed text-background">
            Zamansız siyah-beyaz parçalar. Hesabınızla siparişlerinizi takip edin ve
            favorilerinizi kaydedin.
          </p>
        </div>
      </div>

      {/* Form taraf */}
      <div className="flex items-center justify-center px-4 py-16 md:px-10">
        <div className="w-full max-w-sm">
          <Link href="/" className="mb-10 inline-block font-brand text-2xl font-normal tracking-[0.15em] md:hidden">
            Trendora
          </Link>

          <div className="mb-8">
            <p className="text-xs uppercase tracking-widest text-muted-foreground">
              {isLogin ? "Tekrar hoş geldiniz" : "Aramıza katılın"}
            </p>
            <h1 className="mt-2 font-serif text-4xl font-medium tracking-tight">
              {isLogin ? "Giriş Yap" : "Kayıt Ol"}
            </h1>
          </div>

          <form onSubmit={handleSubmit} className="space-y-5">
            {!isLogin && (
              <div className="grid grid-cols-2 gap-3">
                <Field label="Ad">
                  <input
                    type="text"
                    required
                    autoComplete="given-name"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    placeholder="Adınız"
                    className="w-full border border-border bg-background px-4 py-3 text-sm outline-none transition-colors focus:border-foreground"
                  />
                </Field>
                <Field label="Soyad">
                  <input
                    type="text"
                    required
                    autoComplete="family-name"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    placeholder="Soyadınız"
                    className="w-full border border-border bg-background px-4 py-3 text-sm outline-none transition-colors focus:border-foreground"
                  />
                </Field>
              </div>
            )}

            <Field label="E-posta">
              <input
                type="email"
                required
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="ornek@mail.com"
                className="w-full border border-border bg-background px-4 py-3 text-sm outline-none transition-colors focus:border-foreground"
              />
            </Field>

            <Field
              label="Şifre"
              action={
                isLogin ? (
                  <Link href="/forgot-password" className="text-xs text-muted-foreground underline-offset-4 hover:underline">
                    Şifremi unuttum
                  </Link>
                ) : undefined
              }
            >
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  required
                  minLength={6}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  autoComplete={isLogin ? "current-password" : "new-password"}
                  placeholder="••••••••"
                  className="w-full border border-border bg-background px-4 py-3 pr-11 text-sm outline-none transition-colors focus:border-foreground"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((v) => !v)}
                  aria-label={showPassword ? "Şifreyi gizle" : "Şifreyi göster"}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground transition-colors hover:text-foreground"
                >
                  {showPassword ? <EyeOff className="h-4 w-4" strokeWidth={1.5} /> : <Eye className="h-4 w-4" strokeWidth={1.5} />}
                </button>
              </div>
            </Field>

            {!isLogin && (
              <label className="flex items-start gap-2 text-xs text-muted-foreground">
                <input type="checkbox" required className="mt-0.5 accent-foreground" />
                <span>
                  <Link href="#" className="underline underline-offset-2 hover:text-foreground">
                    Kullanım koşullarını
                  </Link>{" "}
                  ve{" "}
                  <Link href="#" className="underline underline-offset-2 hover:text-foreground">
                    gizlilik politikasını
                  </Link>{" "}
                  kabul ediyorum.
                </span>
              </label>
            )}

            {error && <FormMessage tone="error">{error}</FormMessage>}

            <button
              type="submit"
              disabled={submitting}
              className="w-full bg-foreground py-3 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-90 disabled:opacity-60"
            >
              {submitting ? "Lütfen bekleyin…" : isLogin ? "Giriş Yap" : "Hesap Oluştur"}
            </button>
          </form>

          <div className="my-8 flex items-center gap-4">
            <span className="h-px flex-1 bg-border" />
            <span className="text-xs uppercase tracking-widest text-muted-foreground">veya</span>
            <span className="h-px flex-1 bg-border" />
          </div>

          <p className="text-center text-sm text-muted-foreground">
            {isLogin ? "Henüz hesabınız yok mu? " : "Zaten hesabınız var mı? "}
            <Link
              href={isLogin ? "/register" : "/login"}
              className="font-medium text-foreground underline underline-offset-4"
            >
              {isLogin ? "Kayıt olun" : "Giriş yapın"}
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}

function Field({
  label,
  action,
  children,
}: {
  label: string
  action?: React.ReactNode
  children: React.ReactNode
}) {
  return (
    <div>
      <div className="mb-2 flex items-center justify-between">
        <label className="text-xs uppercase tracking-widest text-muted-foreground">{label}</label>
        {action}
      </div>
      {children}
    </div>
  )
}
