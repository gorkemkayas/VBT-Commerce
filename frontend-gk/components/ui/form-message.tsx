import { AlertCircle, CheckCircle2 } from "lucide-react"
import { cn } from "@/lib/utils"

export function FormMessage({ tone, children }: { tone: "error" | "success"; children: React.ReactNode }) {
  const Icon = tone === "error" ? AlertCircle : CheckCircle2
  return (
    <div
      className={cn(
        "flex items-start gap-2 border px-4 py-3 text-sm",
        tone === "error"
          ? "border-red-600/30 bg-red-600/5 text-red-700 dark:text-red-400"
          : "border-emerald-600/30 bg-emerald-600/5 text-emerald-700 dark:text-emerald-400",
      )}
    >
      <Icon className="mt-0.5 h-4 w-4 shrink-0" strokeWidth={1.5} />
      <span>{children}</span>
    </div>
  )
}
