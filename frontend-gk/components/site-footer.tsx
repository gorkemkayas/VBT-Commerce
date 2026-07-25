"use client"

import type React from "react"
import { useState } from "react"

const columns = [
  { title: "Alışveriş", links: ["Kadın", "Erkek", "Yeni Gelenler", "Koleksiyon"] },
  { title: "Yardım", links: ["Kargo & Teslimat", "İade & Değişim", "Beden Rehberi", "SSS"] },
  { title: "Kurumsal", links: ["Hakkımızda", "Mağazalar", "Kariyer", "Sürdürülebilirlik"] },
]

export function SiteFooter() {
  const [email, setEmail] = useState("")
  const [sent, setSent] = useState(false)

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (email) setSent(true)
  }

  return (
    <footer className="bg-primary text-primary-foreground">
      <div className="px-4 md:px-10">
        <div className="grid grid-cols-1 gap-12 py-16 md:grid-cols-2 lg:grid-cols-4">
          {/* Newsletter */}
          <div className="lg:col-span-1">
            <h2 className="font-brand text-2xl font-normal tracking-[0.15em]">Trendora</h2>
            <p className="mt-4 text-sm leading-relaxed text-primary-foreground/70">
              Yeni koleksiyonlar ve özel kampanyalardan ilk sen haberdar ol.
            </p>
            <form onSubmit={handleSubmit} className="mt-6">
              {sent ? (
                <p className="text-sm text-primary-foreground/80">Teşekkürler, kaydını aldık.</p>
              ) : (
                <div className="flex border border-primary-foreground/30">
                  <input
                    type="email"
                    required
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="E-posta adresin"
                    aria-label="E-posta adresi"
                    className="w-full bg-transparent px-4 py-3 text-sm text-primary-foreground placeholder:text-primary-foreground/50 focus:outline-none"
                  />
                  <button
                    type="submit"
                    className="shrink-0 bg-primary-foreground px-5 text-xs font-medium uppercase tracking-widest text-primary transition-opacity hover:opacity-80"
                  >
                    Kayıt
                  </button>
                </div>
              )}
            </form>
          </div>

          {/* Link columns */}
          {columns.map((col) => (
            <div key={col.title}>
              <h3 className="text-xs font-medium uppercase tracking-widest text-primary-foreground/60">
                {col.title}
              </h3>
              <ul className="mt-5 flex flex-col gap-3">
                {col.links.map((link) => (
                  <li key={link}>
                    <a
                      href="#"
                      className="text-sm text-primary-foreground/90 transition-opacity hover:opacity-60"
                    >
                      {link}
                    </a>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>

        <div className="flex flex-col items-center justify-between gap-4 border-t border-primary-foreground/20 py-6 sm:flex-row">
          <p className="text-xs text-primary-foreground/60">
            © {new Date().getFullYear()} Trendora. Tüm hakları saklıdır.
          </p>
          <div className="flex gap-6 text-xs text-primary-foreground/60">
            <a href="#" className="transition-opacity hover:opacity-100">
              Gizlilik
            </a>
            <a href="#" className="transition-opacity hover:opacity-100">
              Şartlar
            </a>
            <a href="#" className="transition-opacity hover:opacity-100">
              Çerezler
            </a>
          </div>
        </div>
      </div>
    </footer>
  )
}
