import { Package, PackageCheck, Truck, CheckCircle2, XCircle } from "lucide-react"
import type { ShipmentStatus, ShipmentStatusHistoryEntry } from "@/lib/api/types"
import { shipmentStatusLabels } from "@/lib/enum-labels"
import { formatDateTime } from "@/lib/format"

const shipmentStatusIcon: Record<ShipmentStatus, { icon: typeof Package; animationClassName: string }> = {
  Pending: { icon: Package, animationClassName: "animate-[shipment-packing_0.9s_ease-in-out_infinite]" },
  Shipped: { icon: PackageCheck, animationClassName: "animate-[shipment-handoff_1.4s_ease-in-out_infinite]" },
  InTransit: { icon: Truck, animationClassName: "animate-[shipment-drive_1.2s_ease-in-out_infinite]" },
  Delivered: { icon: CheckCircle2, animationClassName: "animate-[shipment-pop_0.5s_ease-out]" },
  Cancelled: { icon: XCircle, animationClassName: "" },
}

export function ShipmentTimeline({ history }: { history: ShipmentStatusHistoryEntry[] }) {
  if (history.length === 0) return null

  return (
    <ol>
      {history.map((entry, i) => {
        const isLast = i === history.length - 1
        const { icon: Icon, animationClassName } = shipmentStatusIcon[entry.status]
        return (
          <li key={i} className="flex gap-4">
            <div className="flex flex-col items-center">
              <span className={`h-2.5 w-2.5 shrink-0 rounded-full ${isLast ? "bg-foreground" : "border border-foreground bg-background"}`} />
              {!isLast && <span className="w-px flex-1 bg-border" />}
            </div>
            <div className={isLast ? "pb-1" : "pb-6"}>
              <div className="flex items-center gap-2">
                <Icon className={`h-4 w-4 shrink-0 ${isLast ? `text-foreground ${animationClassName}` : "text-muted-foreground"}`} aria-hidden />
                <p className={`text-sm ${isLast ? "font-medium text-foreground" : "text-muted-foreground"}`}>
                  {shipmentStatusLabels[entry.status]}
                </p>
              </div>
              <p className="mt-0.5 text-xs text-muted-foreground">{formatDateTime(entry.createdAt)}</p>
              {entry.trackingNumber && <p className="mt-0.5 text-xs text-muted-foreground">Takip No: {entry.trackingNumber}</p>}
            </div>
          </li>
        )
      })}
    </ol>
  )
}
