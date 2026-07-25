"use client"

import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from "react"
import { useAuth } from "@/lib/auth-context"
import { getOrCreateAnonymousId } from "@/lib/anonymous-id"
import * as cartApi from "@/lib/api/cart"
import type { SellableItemType } from "@/lib/api/types"
import { cacheSellableItems, getCachedSellableItem, type CachedSellableItem } from "@/lib/product-cache"

export type CartItem = {
  id: string
  sellableItemId: string
  sellableItemType: SellableItemType
  quantity: number
  productId: string
  name: string
  slug: string
  image: string | null
  variantLabel: string | null
  unitPrice: number | null
}

type AddItemInput = {
  sellableItemId: string
  sellableItemType: SellableItemType
  quantity?: number
  meta: Omit<CachedSellableItem, "id">
}

type CartContextValue = {
  items: CartItem[]
  count: number
  subtotal: number
  loading: boolean
  addItem: (input: AddItemInput) => Promise<void>
  removeItem: (itemId: string) => Promise<void>
  updateQuantity: (itemId: string, quantity: number) => Promise<void>
  clear: () => Promise<void>
  isOpen: boolean
  setOpen: (open: boolean) => void
  refresh: () => Promise<void>
}

const CartContext = createContext<CartContextValue | null>(null)

export function CartProvider({ children }: { children: ReactNode }) {
  const { isAuthenticated, bootstrapping } = useAuth()
  const [anonymousId, setAnonymousId] = useState("")
  const [items, setItems] = useState<CartItem[]>([])
  const [loading, setLoading] = useState(true)
  const [isOpen, setOpen] = useState(false)

  useEffect(() => {
    setAnonymousId(getOrCreateAnonymousId())
  }, [])

  const load = useCallback(async () => {
    if (bootstrapping) return
    if (!isAuthenticated && !anonymousId) return

    setLoading(true)
    try {
      const cart = isAuthenticated ? await cartApi.getMyCart() : await cartApi.getAnonymousCart(anonymousId)
      setItems(
        cart.items.map((item) => {
          const meta = getCachedSellableItem(item.sellableItemId)
          return {
            id: item.id,
            sellableItemId: item.sellableItemId,
            sellableItemType: item.sellableItemType,
            quantity: item.quantity,
            productId: meta?.productId ?? item.sellableItemId,
            name: meta?.productName ?? "Ürün",
            slug: meta?.productSlug ?? "",
            image: meta?.image ?? null,
            variantLabel: meta?.variantLabel ?? null,
            unitPrice: meta?.unitPrice ?? null,
          }
        }),
      )
    } catch {
      setItems([])
    } finally {
      setLoading(false)
    }
  }, [isAuthenticated, anonymousId, bootstrapping])

  useEffect(() => {
    load()
  }, [load])

  async function addItem(input: AddItemInput) {
    cacheSellableItems([{ id: input.sellableItemId, ...input.meta }])
    if (isAuthenticated) {
      await cartApi.addMyCartItem(input.sellableItemId, input.sellableItemType, input.quantity ?? 1)
    } else {
      await cartApi.addAnonymousCartItem(anonymousId, input.sellableItemId, input.sellableItemType, input.quantity ?? 1)
    }
    await load()
    setOpen(true)
  }

  async function removeItem(itemId: string) {
    if (isAuthenticated) await cartApi.removeMyCartItem(itemId)
    else await cartApi.removeAnonymousCartItem(anonymousId, itemId)
    await load()
  }

  async function updateQuantity(itemId: string, quantity: number) {
    if (quantity < 1) return
    if (isAuthenticated) await cartApi.updateMyCartItem(itemId, quantity)
    else await cartApi.updateAnonymousCartItem(anonymousId, itemId, quantity)
    await load()
  }

  async function clear() {
    if (isAuthenticated) await cartApi.clearMyCart()
    else await cartApi.clearAnonymousCart(anonymousId)
    await load()
  }

  const count = items.reduce((sum, i) => sum + i.quantity, 0)
  const subtotal = items.reduce((sum, i) => sum + (i.unitPrice ?? 0) * i.quantity, 0)

  return (
    <CartContext.Provider
      value={{ items, count, subtotal, loading, addItem, removeItem, updateQuantity, clear, isOpen, setOpen, refresh: load }}
    >
      {children}
    </CartContext.Provider>
  )
}

export function useCart() {
  const ctx = useContext(CartContext)
  if (!ctx) throw new Error("useCart must be used within CartProvider")
  return ctx
}
