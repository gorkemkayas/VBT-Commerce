import { Tag } from "lucide-react"
import { getActiveCoupons } from "@/lib/api/pricing"
import type { Coupon } from "@/lib/api/types"
import { formatPrice } from "@/lib/format"

function formatCoupon(coupon: Coupon) {
  const discount = coupon.discountType === "Percentage" ? `%${coupon.discountValue} indirim` : `${formatPrice(coupon.discountValue)} indirim`
  const condition = coupon.minCartAmount ? ` · ${formatPrice(coupon.minCartAmount)} ve üzeri alışverişlerde` : ""
  return `${coupon.code} ile ${discount}${condition}`
}

export async function CouponMarquee() {
  const coupons = await getActiveCoupons().catch(() => [])

  if (coupons.length === 0) return null

  const items = coupons.map(formatCoupon)
  // Repeated enough times that the track stays wider than any viewport — otherwise, with only
  // a couple of coupons, the 2x-duplicated track would run out of content mid-scroll and leave a
  // visible gap instead of appearing to flow in continuously from the right edge.
  const repeated = Array.from({ length: Math.max(1, Math.ceil(16 / items.length)) }, () => items).flat()
  const track = [...repeated, ...repeated]

  return (
    <div className="overflow-hidden border-b border-border bg-foreground py-2.5 text-background">
      <div className="flex w-max animate-[marquee_60s_linear_infinite]">
        {track.map((text, i) => (
          <span
            key={i}
            aria-hidden={i >= repeated.length}
            className="flex shrink-0 items-center gap-2 px-8 text-xs font-medium uppercase tracking-widest"
          >
            <Tag className="h-3.5 w-3.5" strokeWidth={1.5} aria-hidden />
            {text}
          </span>
        ))}
      </div>
    </div>
  )
}
