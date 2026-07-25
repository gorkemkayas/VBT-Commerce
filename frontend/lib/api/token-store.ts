// Access token'ı bellekte tutan, sayfa yenilemede localStorage'dan geri yükleyen ve
// abone olanlara değişiklikleri bildiren küçük bir store. Refresh token web'de HttpOnly
// çerezde tutulduğu için burada hiç görünmez; JS'in tek görevi access token'ı taşımak.

export type DecodedUser = { userId: string; email: string; firstName: string; lastName: string; role: "Customer" | "Admin" }

type Snapshot = {
  accessToken: string | null
  accessTokenExpiresAt: number | null // epoch ms
  user: DecodedUser | null
}

const STORAGE_KEY = "vbt-access-token"

let snapshot: Snapshot = { accessToken: null, accessTokenExpiresAt: null, user: null }
const listeners = new Set<() => void>()

function decodeJwt(token: string): DecodedUser | null {
  try {
    const payload = token.split(".")[1]
    const json = JSON.parse(decodeURIComponent(escape(atob(payload.replace(/-/g, "+").replace(/_/g, "/")))))
    const role = json["role"] ?? json["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
    const userId = json["sub"] ?? json["nameid"] ?? json["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
    const email = json["email"] ?? json["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]
    const firstName = json["given_name"] ?? json["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"]
    const lastName = json["family_name"] ?? json["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"]
    if (!role || !userId) return null
    return { userId, email: email ?? "", firstName: firstName ?? "", lastName: lastName ?? "", role: role === "Admin" ? "Admin" : "Customer" }
  } catch {
    return null
  }
}

function persist() {
  if (typeof window === "undefined") return
  try {
    if (snapshot.accessToken) {
      window.localStorage.setItem(
        STORAGE_KEY,
        JSON.stringify({ accessToken: snapshot.accessToken, accessTokenExpiresAt: snapshot.accessTokenExpiresAt }),
      )
    } else {
      window.localStorage.removeItem(STORAGE_KEY)
    }
  } catch {
    // yoksay
  }
}

export function loadPersistedToken() {
  if (typeof window === "undefined") return
  try {
    const raw = window.localStorage.getItem(STORAGE_KEY)
    if (!raw) return
    const parsed = JSON.parse(raw) as { accessToken: string; accessTokenExpiresAt: number }
    if (parsed.accessTokenExpiresAt > Date.now()) {
      setToken(parsed.accessToken, parsed.accessTokenExpiresAt)
    } else {
      window.localStorage.removeItem(STORAGE_KEY)
    }
  } catch {
    // yoksay
  }
}

export function setToken(accessToken: string, accessTokenExpiresAt: number) {
  snapshot = { accessToken, accessTokenExpiresAt, user: decodeJwt(accessToken) }
  persist()
  listeners.forEach((l) => l())
}

export function clearToken() {
  snapshot = { accessToken: null, accessTokenExpiresAt: null, user: null }
  persist()
  listeners.forEach((l) => l())
}

export function getSnapshot(): Snapshot {
  return snapshot
}

export function isTokenNearExpiry(bufferMs = 15_000) {
  if (!snapshot.accessTokenExpiresAt) return true
  return snapshot.accessTokenExpiresAt - Date.now() < bufferMs
}

export function subscribe(listener: () => void) {
  listeners.add(listener)
  return () => listeners.delete(listener)
}
