import { Analytics } from '@vercel/analytics/next'
import type { Metadata, Viewport } from 'next'
import { Geist, Geist_Mono, Jaini, Playfair_Display } from 'next/font/google'
import { AuthProvider } from '@/lib/auth-context'
import { CartProvider } from '@/components/cart-provider'
import { CartDrawer } from '@/components/cart-drawer'
import './globals.css'

// latin-ext is included so the Turkish Lira sign (₺, U+20BA) is served by these fonts themselves —
// without it, the browser silently falls back to a system font for just that glyph, which renders
// it at a mismatched (visibly larger) size next to the digits.
const geistSans = Geist({ subsets: ['latin', 'latin-ext'], variable: '--font-geist-sans' })
const geistMono = Geist_Mono({ subsets: ['latin', 'latin-ext'], variable: '--font-geist-mono' })
const playfairDisplay = Playfair_Display({ subsets: ['latin', 'latin-ext'], variable: '--font-display' })
const jaini = Jaini({ subsets: ['latin', 'latin-ext'], weight: '400', variable: '--font-brand' })

export const metadata: Metadata = {
  title: 'Trendora',
  description: 'Editorial monochrome fashion. Timeless black and white essentials.',
  generator: 'v0.app',
  icons: {
    icon: [
      {
        url: '/icon-light-32x32.png',
        media: '(prefers-color-scheme: light)',
      },
      {
        url: '/icon-dark-32x32.png',
        media: '(prefers-color-scheme: dark)',
      },
      {
        url: '/icon.svg',
        type: 'image/svg+xml',
      },
    ],
    apple: '/apple-icon.png',
  },
}

export const viewport: Viewport = {
  colorScheme: 'light dark',
  themeColor: [
    { media: '(prefers-color-scheme: light)', color: 'white' },
    { media: '(prefers-color-scheme: dark)', color: 'black' },
  ],
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html lang="tr" className={`bg-background ${geistSans.variable} ${geistMono.variable} ${playfairDisplay.variable} ${jaini.variable}`}>
      <body className="font-sans antialiased">
        <AuthProvider>
          <CartProvider>
            {children}
            <CartDrawer />
          </CartProvider>
        </AuthProvider>
        {process.env.NODE_ENV === 'production' && <Analytics />}
      </body>
    </html>
  )
}
