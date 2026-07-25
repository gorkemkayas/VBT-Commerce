import { PaymentDetail } from "@/components/admin/payments/payment-detail"

export default async function AdminPaymentDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params
  return <PaymentDetail paymentId={id} />
}
