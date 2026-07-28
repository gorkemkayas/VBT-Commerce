export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "https://intern-api.kayas.dev"

// Server-side (SSR) requests use this instead of API_BASE_URL so they hit the API over the
// Docker-internal network rather than round-tripping through the public domain/nginx — that
// hairpin path made every visitor's SSR calls look like they came from one shared IP to the
// backend's per-IP rate limiter, tripping 429s under normal traffic.
export const SERVER_API_BASE_URL = process.env.INTERNAL_API_URL ?? API_BASE_URL
