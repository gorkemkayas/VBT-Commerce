import { ShipmentDetail } from "@/components/admin/shipping/shipment-detail"

export default async function AdminShipmentDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params
  return <ShipmentDetail shipmentId={id} />
}
