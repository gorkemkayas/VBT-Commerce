// Access token'ı yalnızca bellekte tutan ve abone olanlara değişiklikleri bildiren küçük bir
// store. Sayfa yenilemede token, AuthProvider'ın mount'ta çağırdığı tryRestoreSession() ile
// (httpOnly refresh çerezi üzerinden) yeniden alınır — localStorage'a hiç yazılmaz, aksi halde
// kısa ömürlü de olsa access token XSS'e karşı gereksiz yere açıkta kalır. Refresh token zaten
// HttpOnly çerezde tutulduğu için burada hiç görünmez; JS'in tek görevi access token'ı taşımak.

export type DecodedUser = { userId: string; email: string; firstName: string; lastName: string; role: "Customer" | "Admin" }

type Snapshot = {
  accessToken: string | null
  accessTokenExpiresAt: number | null // epoch ms
  user: DecodedUser | null
}

let snapshot: Snapshot = { accessToken: null, accessTokenExpiresAt: null, user: null }
const listeners = new Set<() => void>()

// One-time cleanup: earlier versions persisted the access token to localStorage under this key.
// Nothing reads it anymore, but browsers that still have it stored would otherwise keep an old
// (eventually stale, but needlessly XSS-exposed) token sitting around indefinitely.
if (typeof window !== "undefined") {
  try {
    window.localStorage.removeItem("vbt-access-token")
  } catch {
    // yoksay
  }
}

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

export function setToken(accessToken: string, accessTokenExpiresAt: number) {
  snapshot = { accessToken, accessTokenExpiresAt, user: decodeJwt(accessToken) }
  listeners.forEach((l) => l())
}

export function clearToken() {
  snapshot = { accessToken: null, accessTokenExpiresAt: null, user: null }
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
