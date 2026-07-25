import * as React from "react"
import { cn } from "@/lib/utils"

const tones = {
  neutral: "border-border text-muted-foreground",
  solid: "border-foreground bg-foreground text-background",
  success: "border-emerald-600/40 bg-emerald-600/10 text-emerald-700 dark:text-emerald-400",
  warning: "border-amber-600/40 bg-amber-600/10 text-amber-700 dark:text-amber-400",
  danger: "border-red-600/40 bg-red-600/10 text-red-700 dark:text-red-400",
  info: "border-sky-600/40 bg-sky-600/10 text-sky-700 dark:text-sky-400",
}

export function Badge({
  tone = "neutral",
  className,
  ...props
}: React.ComponentProps<"span"> & { tone?: keyof typeof tones }) {
  return (
    <span
      className={cn(
        "inline-flex items-center gap-1 border px-2.5 py-1 text-[10px] font-medium uppercase tracking-wider",
        tones[tone],
        className,
      )}
      {...props}
    />
  )
}
