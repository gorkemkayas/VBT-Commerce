"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { LogOut, Menu, Search, ShoppingBag, User, UserCog, X } from "lucide-react"
import { useCart } from "@/components/cart-provider"
import { useAuth } from "@/lib/auth-context"

const navItems = [
  { label: "Mağaza", href: "/shop" },
  { label: "Sipariş Sorgula", href: "/track-order" },
]

export function SiteHeader({ overlay = false }: { overlay?: boolean }) {
  const [open, setOpen] = useState(false)
  const [scrolled, setScrolled] = useState(false)
  const { count, setOpen: setCartOpen } = useCart()
  const { isAuthenticated, isAdmin, logout } = useAuth()

  useEffect(() => {
    if (!overlay) return
    function handleScroll() {
      setScrolled(window.scrollY > 60)
    }
    handleScroll()
    window.addEventListener("scroll", handleScroll, { passive: true })
    return () => window.removeEventListener("scroll", handleScroll)
  }, [overlay])

  // Over the hero video, the header starts transparent with light text so the video reads clearly;
  // once scrolled (or on any page without a hero) it's solid with the normal dark text.
  const solid = !overlay || scrolled
  const fg = solid ? "text-foreground" : "text-white"
  const fgMuted = solid ? "text-foreground/70" : "text-white/80"

  return (
    <header
      className={`z-50 transition-colors duration-300 ${overlay ? "fixed inset-x-0 top-0" : "sticky top-0 border-b border-border bg-background"} ${
        overlay ? (solid ? "border-b border-border bg-background" : "border-b border-transparent bg-transparent") : ""
      }`}
    >
      <div className="flex h-14 items-center justify-between px-4 md:px-10">
        <div className="flex items-center gap-5">
          <button
            type="button"
            aria-label="Menüyü aç"
            onClick={() => setOpen(true)}
            className={`md:hidden ${fg}`}
          >
            <Menu className="h-5 w-5" strokeWidth={1.5} />
          </button>
          <nav className="hidden items-center gap-8 md:flex">
            {navItems.map((item) => (
              <Link
                key={item.label}
                href={item.href}
                className={`text-xs font-medium uppercase tracking-widest transition-opacity hover:opacity-60 ${fg}`}
              >
                {item.label}
              </Link>
            ))}
          </nav>
        </div>

        <Link href="/" className="flex flex-col items-center leading-none" aria-label="Trendora ana sayfa">
          <span className={`font-brand text-2xl font-normal tracking-[0.12em] transition-colors ${fg}`}>Trendora</span>
        </Link>

        <div className={`flex items-center gap-5 ${fg}`}>
          <Link href="/shop" aria-label="Ara" className="transition-opacity hover:opacity-60">
            <Search className="h-5 w-5" strokeWidth={1.5} />
          </Link>
          {isAdmin && (
            <Link href="/admin" aria-label="Admin Paneli" title="Admin Paneli" className="transition-opacity hover:opacity-60">
              <UserCog className="h-5 w-5" strokeWidth={1.5} />
            </Link>
          )}
          <Link href={isAuthenticated ? "/account" : "/login"} aria-label="Hesabım" className="transition-opacity hover:opacity-60">
            <User className="h-5 w-5" strokeWidth={1.5} />
          </Link>
          <button
            type="button"
            aria-label="Sepeti aç"
            onClick={() => setCartOpen(true)}
            className="flex items-center gap-1.5 transition-opacity hover:opacity-60"
          >
            <ShoppingBag className="h-5 w-5" strokeWidth={1.5} />
            <span className={`text-xs font-medium tabular-nums ${fgMuted}`}>({count})</span>
          </button>
          {isAuthenticated && (
            <button type="button" aria-label="Çıkış yap" onClick={() => logout()} className="hidden transition-opacity hover:opacity-60 md:block">
              <LogOut className="h-5 w-5" strokeWidth={1.5} />
            </button>
          )}
        </div>
      </div>

      {open && (
        <div className="fixed inset-0 z-50 bg-background md:hidden">
          <div className="flex h-16 items-center justify-between border-b border-border px-4">
            <span className="font-brand text-xl font-normal tracking-[0.15em]">Trendora</span>
            <button type="button" aria-label="Menüyü kapat" onClick={() => setOpen(false)}>
              <X className="h-5 w-5" strokeWidth={1.5} />
            </button>
          </div>
          <nav className="flex flex-col">
            {navItems.map((item) => (
              <Link
                key={item.label}
                href={item.href}
                onClick={() => setOpen(false)}
                className="border-b border-border px-4 py-5 text-sm font-medium uppercase tracking-widest"
              >
                {item.label}
              </Link>
            ))}
            {isAuthenticated && (
              <button
                type="button"
                onClick={() => {
                  setOpen(false)
                  logout()
                }}
                className="border-b border-border px-4 py-5 text-left text-sm font-medium uppercase tracking-widest"
              >
                Çıkış Yap
              </button>
            )}
          </nav>
        </div>
      )}
    </header>
  )
}
