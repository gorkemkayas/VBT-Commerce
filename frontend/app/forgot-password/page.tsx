import { ForgotPasswordForm } from "@/components/forgot-password-form"

export const metadata = { title: "Şifremi Unuttum — Trendora" }

export default function ForgotPasswordPage() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-4 text-foreground">
      <ForgotPasswordForm />
    </main>
  )
}
