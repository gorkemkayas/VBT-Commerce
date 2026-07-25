"use client"

import Image from "next/image"
import Link from "next/link"
import { Minus, Plus, X } from "lucide-react"
import { useCart } from "@/components/cart-provider"
import { formatPrice } from "@/lib/format"

export function CartDrawer() {
  const { items, subtotal, isOpen, setOpen, updateQuantity, removeItem } = useCart()

  return (
    <>
      <div
        aria-hidden={!isOpen}
        onClick={() => setOpen(false)}
        className={`fixed inset-0 z-[60] bg-foreground/40 transition-opacity duration-300 ${
          isOpen ? "opacity-100" : "pointer-events-none opacity-0"
        }`}
      />
      <aside
        role="dialog"
        aria-label="Sepet"
        aria-modal="true"
        className={`fixed right-0 top-0 z-[70] flex h-full w-full max-w-md flex-col border-l border-border bg-background transition-transform duration-300 ${
          isOpen ? "translate-x-0" : "translate-x-full"
        }`}
      >
        <div className="flex items-center justify-between border-b border-border px-5 py-4">
          <h2 className="text-sm font-medium uppercase tracking-widest">
            Sepet ({items.reduce((s, i) => s + i.quantity, 0)})
          </h2>
          <button type="button" aria-label="Sepeti kapat" onClick={() => setOpen(false)}>
            <X className="h-5 w-5" strokeWidth={1.5} />
          </button>
        </div>

        {items.length === 0 ? (
          <div className="flex flex-1 flex-col items-center justify-center gap-4 px-6 text-center">
            <p className="text-sm text-muted-foreground">Sepetiniz boş.</p>
            <button
              type="button"
              onClick={() => setOpen(false)}
              className="border border-foreground px-6 py-3 text-xs font-medium uppercase tracking-widest transition-colors hover:bg-foreground hover:text-background"
            >
              Alışverişe Devam Et
            </button>
          </div>
        ) : (
          <>
            <div className="flex-1 divide-y divide-border overflow-y-auto">
              {items.map((item) => (
                <div key={item.id} className="flex gap-4 p-5">
                  <div className="relative aspect-[3/4] w-20 shrink-0 bg-secondary">
                    <Image
                      src={item.image || "/placeholder.svg"}
                      alt={item.name}
                      fill
                      sizes="80px"
                      className="object-cover"
                    />
                  </div>
                  <div className="flex flex-1 flex-col justify-between">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <h3 className="text-sm font-medium">{item.name}</h3>
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
                          className="flex h-8 w-8 items-center justify-center transition-colors hover:bg-secondary"
                        >
                          <Minus className="h-3 w-3" strokeWidth={1.5} />
                        </button>
                        <span className="w-8 text-center text-xs tabular-nums">{item.quantity}</span>
                        <button
                          type="button"
                          aria-label="Artır"
                          onClick={() => updateQuantity(item.id, item.quantity + 1)}
                          className="flex h-8 w-8 items-center justify-center transition-colors hover:bg-secondary"
                        >
                          <Plus className="h-3 w-3" strokeWidth={1.5} />
                        </button>
                      </div>
                      <p className="font-mono text-sm">{formatPrice((item.unitPrice ?? 0) * item.quantity)}</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            <div className="border-t border-border p-5">
              <div className="mb-4 flex items-center justify-between">
                <span className="text-xs uppercase tracking-widest text-muted-foreground">Ara Toplam</span>
                <span className="font-mono text-base">{formatPrice(subtotal)}</span>
              </div>
              <Link
                href="/cart"
                onClick={() => setOpen(false)}
                className="block w-full bg-foreground py-4 text-center text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-80"
              >
                Sepete Git
              </Link>
            </div>
          </>
        )}
      </aside>
    </>
  )
}
