import { SiteHeader } from "@/components/site-header"
import { SiteFooter } from "@/components/site-footer"
import { ProfileView } from "@/components/profile-view"

export const dynamic = "force-dynamic"

export const metadata = {
  title: "Hesabım — Trendora",
  description: "Siparişlerinizi, kargo sürecini ve kişisel bilgilerinizi yönetin.",
}

export default function AccountPage() {
  return (
    <main className="min-h-screen bg-background text-foreground">
      <SiteHeader />
      <ProfileView />
      <SiteFooter />
    </main>
  )
}
