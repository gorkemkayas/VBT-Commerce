"use client"

import Image from "next/image"
import Link from "next/link"
import { Minus, Plus, X } from "lucide-react"
import { useCart } from "@/components/cart-provider"
import { formatPrice } from "@/lib/format"
import { PageSpinner } from "@/components/ui/spinner"

export function CartView() {
  const { items, subtotal, updateQuantity, removeItem, clear, loading } = useCart()

  if (loading && items.length === 0) return <PageSpinner label="Sepet yükleniyor…" />

  if (items.length === 0) {
    return (
      <div className="mx-auto flex min-h-[60vh] max-w-7xl flex-col items-center justify-center px-4 text-center">
        <h1 className="font-serif text-3xl font-medium tracking-tight">Sepetiniz Boş</h1>
        <p className="mt-3 text-sm text-muted-foreground">Koleksiyonu keşfedin ve favorilerinizi ekleyin.</p>
        <Link
          href="/shop"
          className="mt-8 border border-foreground px-8 py-4 text-xs font-medium uppercase tracking-widest transition-colors hover:bg-foreground hover:text-background"
        >
          Alışverişe Başla
        </Link>
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-7xl px-4 md:px-6">
      <div className="flex items-end justify-between border-b border-border py-10">
        <h1 className="font-serif text-4xl font-medium tracking-tight md:text-5xl">Sepet</h1>
        <button
          type="button"
          onClick={() => clear()}
          className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground"
        >
          Sepeti Boşalt
        </button>
      </div>

      <div className="grid grid-cols-1 gap-10 py-10 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <ul className="divide-y divide-border border-y border-border">
            {items.map((item) => (
              <li key={item.id} className="flex gap-5 py-6">
                <Link href={item.slug ? `/product/${item.slug}` : "#"} className="relative aspect-[3/4] w-24 shrink-0 bg-secondary">
                  <Image
                    src={item.image || "/placeholder.svg"}
                    alt={item.name}
                    fill
                    sizes="96px"
                    className="object-cover"
                  />
                </Link>
                <div className="flex flex-1 flex-col justify-between">
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <Link href={item.slug ? `/product/${item.slug}` : "#"} className="text-base font-medium hover:opacity-60">
                        {item.name}
                      </Link>
                      {item.variantLabel && (
                        <p className="mt-1 text-[10px] uppercase tracking-widest text-muted-foreground">{item.variantLabel}</p>
                      )}
                    </div>
                    <button
                      type="button"
                      aria-label="Ürünü kaldır"
                      onClick={() => removeItem(item.id)}
                      className="text-muted-foreground transition-colors hover:text-foreground"
                    >
                      <X className="h-4 w-4" strokeWidth={1.5} />
                    </button>
                  </div>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center border border-border">
                      <button
                        type="button"
                        aria-label="Azalt"
                        onClick={() => updateQuantity(item.id, item.quantity - 1)}
                        className="flex h-9 w-9 items-center justify-center transition-colors hover:bg-secondary"
                      >
                        <Minus className="h-3 w-3" strokeWidth={1.5} />
                      </button>
                      <span className="w-9 text-center text-xs tabular-nums">{item.quantity}</span>
                      <button
                        type="button"
                        aria-label="Artır"
                        onClick={() => updateQuantity(item.id, item.quantity + 1)}
                        className="flex h-9 w-9 items-center justify-center transition-colors hover:bg-secondary"
                      >
                        <Plus className="h-3 w-3" strokeWidth={1.5} />
                      </button>
                    </div>
                    <p className="font-mono text-base">{formatPrice((item.unitPrice ?? 0) * item.quantity)}</p>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>

        <aside className="h-fit border border-border p-6 lg:sticky lg:top-24">
          <h2 className="text-sm font-medium uppercase tracking-widest">Sipariş Özeti</h2>
          <dl className="mt-6 space-y-4 border-b border-border pb-6 text-sm">
            <div className="flex justify-between">
              <dt className="text-muted-foreground">Ara Toplam</dt>
              <dd className="font-mono">{formatPrice(subtotal)}</dd>
            </div>
            <div className="flex justify-between">
              <dt className="text-muted-foreground">Kargo &amp; Vergi</dt>
              <dd className="font-mono text-xs text-muted-foreground">Ödeme adımında hesaplanır</dd>
            </div>
          </dl>
          <div className="mt-6 flex items-baseline justify-between">
            <span className="text-sm font-medium uppercase tracking-widest">Ara Toplam</span>
            <span className="font-mono text-xl">{formatPrice(subtotal)}</span>
          </div>
          <Link
            href="/checkout"
            className="mt-6 block w-full bg-foreground py-4 text-center text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-80"
          >
            Ödemeye Geç
          </Link>
          <Link
            href="/shop"
            className="mt-3 block w-full py-3 text-center text-[10px] font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground"
          >
            Alışverişe Devam Et
          </Link>
        </aside>
      </div>
    </div>
  )
}
