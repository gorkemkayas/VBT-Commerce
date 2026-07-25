const STORAGE_KEY = "vbt-anonymous-id"

export function getOrCreateAnonymousId(): string {
  if (typeof window === "undefined") return ""
  let id = window.localStorage.getItem(STORAGE_KEY)
  if (!id) {
    id = crypto.randomUUID()
    window.localStorage.setItem(STORAGE_KEY, id)
  }
  return id
}

export function resetAnonymousId(): string {
  const id = crypto.randomUUID()
  window.localStorage.setItem(STORAGE_KEY, id)
  return id
}
