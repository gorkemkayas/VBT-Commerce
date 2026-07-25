import { EditCouponClient } from "@/components/admin/coupons/edit-coupon-client"

export default async function EditCouponPage({ params }: { params: Promise<{ code: string }> }) {
  const { code } = await params
  return <EditCouponClient code={code} />
}
