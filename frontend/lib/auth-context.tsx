"use client"

import { createContext, useContext, useEffect, useState, type ReactNode } from "react"
import { useSyncExternalStore } from "react"
import * as authApi from "@/lib/api/auth"
import { tryRestoreSession } from "@/lib/api/client"
import { getSnapshot, subscribe, type DecodedUser } from "@/lib/api/token-store"
import { getOrCreateAnonymousId } from "@/lib/anonymous-id"
import { ApiError } from "@/lib/api/client"

type AuthContextValue = {
  user: DecodedUser | null
  isAuthenticated: boolean
  isAdmin: boolean
  bootstrapping: boolean
  login: (email: string, password: string) => Promise<void>
  register: (input: { email: string; password: string; firstName: string; lastName: string }) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const snapshot = useSyncExternalStore(
    subscribe,
    () => getSnapshot(),
    () => getSnapshot(),
  )
  const [bootstrapping, setBootstrapping] = useState(true)

  useEffect(() => {
    let cancelled = false
    tryRestoreSession().finally(() => {
      if (!cancelled) setBootstrapping(false)
    })
    return () => {
      cancelled = true
    }
  }, [])

  async function login(email: string, password: string) {
    await authApi.login({ email, password, anonymousId: getOrCreateAnonymousId() })
  }

  async function register(input: { email: string; password: string; firstName: string; lastName: string }) {
    await authApi.register({ ...input, anonymousId: getOrCreateAnonymousId() })
  }

  async function logout() {
    await authApi.logout()
  }

  const value: AuthContextValue = {
    user: snapshot.user,
    isAuthenticated: snapshot.user !== null,
    isAdmin: snapshot.user?.role === "Admin",
    bootstrapping,
    login,
    register,
    logout,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error("useAuth must be used within AuthProvider")
  return ctx
}

export function extractErrorMessage(error: unknown, fallback: string) {
  if (error instanceof ApiError) return error.message
  return fallback
}
