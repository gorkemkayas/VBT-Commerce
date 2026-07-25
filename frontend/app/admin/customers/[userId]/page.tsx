import { CustomerDetail } from "@/components/admin/customers/customer-detail"

export default async function AdminCustomerDetailPage({ params }: { params: Promise<{ userId: string }> }) {
  const { userId } = await params
  return <CustomerDetail userId={userId} />
}
