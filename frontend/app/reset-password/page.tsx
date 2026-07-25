import { ResetPasswordForm } from "@/components/reset-password-form"

export const dynamic = "force-dynamic"

export const metadata = { title: "Şifre Sıfırla — Trendora" }

export default function ResetPasswordPage() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-4 text-foreground">
      <ResetPasswordForm />
    </main>
  )
}
