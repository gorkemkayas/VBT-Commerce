import { API_BASE_URL, SERVER_API_BASE_URL } from "./config"
import { clearToken, getSnapshot, isTokenNearExpiry, setToken } from "./token-store"
import type { ApiProblem } from "./types"

export class ApiError extends Error {
  status: number
  problem: ApiProblem | null

  constructor(status: number, message: string, problem: ApiProblem | null) {
    super(message)
    this.name = "ApiError"
    this.status = status
    this.problem = problem
  }
}

type RequestOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE"
  body?: unknown
  query?: Record<string, string | number | boolean | null | undefined>
  auth?: boolean // Authorization header eklensin mi (varsayılan: true)
  signal?: AbortSignal
}

function buildUrl(path: string, query?: RequestOptions["query"]) {
  const base = typeof window === "undefined" ? SERVER_API_BASE_URL : API_BASE_URL
  const url = new URL(path.startsWith("http") ? path : `${base}${path}`)
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      if (value === null || value === undefined || value === "") continue
      url.searchParams.set(key, String(value))
    }
  }
  return url.toString()
}

let refreshPromise: Promise<boolean> | null = null

// Sekmeler arası/ard arda tekrarlanan çağrılarda tek bir refresh isteği paylaşılır.
async function refreshAccessToken(): Promise<boolean> {
  if (!refreshPromise) {
    refreshPromise = (async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
          method: "POST",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({}),
        })
        if (!res.ok) {
          clearToken()
          return false
        }
        const data = await res.json()
        setToken(data.accessToken, new Date(data.accessTokenExpiresAt).getTime())
        return true
      } catch {
        return false
      } finally {
        refreshPromise = null
      }
    })()
  }
  return refreshPromise
}

async function parseProblem(res: Response): Promise<ApiProblem | null> {
  try {
    const text = await res.text()
    if (!text) return null
    return JSON.parse(text) as ApiProblem
  } catch {
    return null
  }
}

function problemMessage(problem: ApiProblem | null, fallback: string) {
  if (!problem) return fallback
  if (problem.errors) {
    const messages = Object.values(problem.errors).flat()
    if (messages.length) return messages.join(" ")
  }
  return problem.detail || problem.title || fallback
}

async function performFetch(path: string, options: RequestOptions, allowRefresh: boolean): Promise<Response> {
  const { method = "GET", body, query, auth = true, signal } = options

  if (auth && isTokenNearExpiry() && getSnapshot().accessToken !== null) {
    await refreshAccessToken()
  }

  const headers: Record<string, string> = {}
  if (body !== undefined) headers["Content-Type"] = "application/json"
  if (auth) {
    const token = getSnapshot().accessToken
    if (token) headers["Authorization"] = `Bearer ${token}`
  }

  const res = await fetch(buildUrl(path, query), {
    method,
    headers,
    credentials: "include",
    body: body !== undefined ? JSON.stringify(body) : undefined,
    signal,
  })

  if (res.status === 401 && auth && allowRefresh && getSnapshot().accessToken) {
    const refreshed = await refreshAccessToken()
    if (refreshed) return performFetch(path, options, false)
  }

  return res
}

export async function apiFetch<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const res = await performFetch(path, options, true)

  if (res.status === 204) return undefined as T

  if (!res.ok) {
    const problem = await parseProblem(res)
    throw new ApiError(res.status, problemMessage(problem, `İstek başarısız oldu (${res.status}).`), problem)
  }

  const text = await res.text()
  if (!text) return undefined as T
  return JSON.parse(text) as T
}

export async function tryRestoreSession(): Promise<void> {
  await refreshAccessToken()
}
