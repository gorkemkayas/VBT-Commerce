import { apiFetch } from "./client"
import { clearToken, setToken } from "./token-store"
import type { AuthResponse, ClientPlatform } from "./types"

function applyAuthResponse(res: AuthResponse) {
  setToken(res.accessToken, new Date(res.accessTokenExpiresAt).getTime())
}

const PLATFORM: ClientPlatform = "Web"

export async function register(input: {
  email: string
  password: string
  firstName: string
  lastName: string
  anonymousId?: string | null
}) {
  const res = await apiFetch<AuthResponse>("/api/auth/register", {
    method: "POST",
    auth: false,
    body: { ...input, platform: PLATFORM },
  })
  applyAuthResponse(res)
  return res
}

export async function login(input: { email: string; password: string; anonymousId?: string | null }) {
  const res = await apiFetch<AuthResponse>("/api/auth/login", {
    method: "POST",
    auth: false,
    body: { ...input, platform: PLATFORM },
  })
  applyAuthResponse(res)
  return res
}

export async function logout() {
  try {
    await apiFetch<void>("/api/auth/logout", { method: "POST", auth: false, body: {} })
  } finally {
    clearToken()
  }
}

export async function forgotPassword(email: string) {
  await apiFetch<void>("/api/auth/forgot-password", { method: "POST", auth: false, body: { email } })
}

export async function resetPassword(token: string, newPassword: string) {
  await apiFetch<void>("/api/auth/reset-password", { method: "POST", auth: false, body: { token, newPassword } })
}
