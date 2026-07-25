import { AuthForm } from "@/components/auth-form"

export const dynamic = "force-dynamic"

export const metadata = {
  title: "Giriş Yap — Trendora",
}

export default function LoginPage() {
  return (
    <main className="min-h-screen bg-background text-foreground">
      <AuthForm mode="login" />
    </main>
  )
}
