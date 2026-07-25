import { Loader2 } from "lucide-react"
import { cn } from "@/lib/utils"

export function Spinner({ className }: { className?: string }) {
  return <Loader2 className={cn("h-4 w-4 animate-spin", className)} strokeWidth={2} />
}

export function PageSpinner({ label = "Yükleniyor…" }: { label?: string }) {
  return (
    <div className="flex min-h-[40vh] flex-col items-center justify-center gap-3 text-muted-foreground">
      <Spinner className="h-6 w-6" />
      <p className="text-xs uppercase tracking-widest">{label}</p>
    </div>
  )
}
