import { AuthForm } from "@/components/auth-form"

export const dynamic = "force-dynamic"

export const metadata = {
  title: "Kayıt Ol — Trendora",
}

export default function RegisterPage() {
  return (
    <main className="min-h-screen bg-background text-foreground">
      <AuthForm mode="register" />
    </main>
  )
}
