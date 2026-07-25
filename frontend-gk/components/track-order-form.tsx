"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { FormMessage } from "@/components/ui/form-message"

export function TrackOrderForm() {
  const router = useRouter()
  const [orderId, setOrderId] = useState("")
  const [guestCustomerId, setGuestCustomerId] = useState("")
  const [error, setError] = useState<string | null>(null)

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    if (!orderId.trim() || !guestCustomerId.trim()) {
      setError("Lütfen sipariş takip referansındaki her iki değeri de girin.")
      return
    }
    router.push(`/checkout/${orderId.trim()}?guestCustomerId=${guestCustomerId.trim()}`)
  }

  return (
    <div className="mx-auto max-w-md px-4 py-20">
      <p className="text-xs uppercase tracking-widest text-muted-foreground">Misafir Sipariş Sorgulama</p>
      <h1 className="mt-2 font-serif text-4xl font-medium tracking-tight">Siparişimi Bul</h1>
      <p className="mt-3 text-sm text-muted-foreground">
        Sipariş onayı sayfasında size verilen takip referansındaki iki değeri girin (sipariş no / müşteri no).
      </p>

      <form onSubmit={handleSubmit} className="mt-8 space-y-5">
        <div>
          <Label>Sipariş No</Label>
          <Input required value={orderId} onChange={(e) => setOrderId(e.target.value)} placeholder="00000000-0000-0000-0000-000000000000" />
        </div>
        <div>
          <Label>Müşteri No</Label>
          <Input required value={guestCustomerId} onChange={(e) => setGuestCustomerId(e.target.value)} placeholder="00000000-0000-0000-0000-000000000000" />
        </div>
        {error && <FormMessage tone="error">{error}</FormMessage>}
        <button type="submit" className="w-full bg-foreground py-3 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-90">
          Siparişi Görüntüle
        </button>
      </form>
    </div>
  )
}
