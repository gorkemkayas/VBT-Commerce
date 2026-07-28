import { NextRequest, NextResponse } from "next/server"
import { getProducts } from "@/lib/api/catalog"
import { withListPrices } from "@/lib/store-catalog"

// Runs entirely server-side, so getProducts/withListPrices reach the API over the Docker-internal
// network (see SERVER_API_BASE_URL) instead of the browser making ~2-3 public API requests per
// product — that fan-out was blowing through the per-IP rate limit on a single /shop page load.
export async function GET(request: NextRequest) {
  const { searchParams } = new URL(request.url)
  const categoryId = searchParams.get("categoryId") ?? undefined
  const searchTerm = searchParams.get("searchTerm") ?? undefined

  const { items } = await getProducts({
    pageNumber: 1,
    pageSize: 48,
    isActive: true,
    categoryId,
    searchTerm,
  })
  const withPrices = await withListPrices(items)

  return NextResponse.json({ items: withPrices })
}
