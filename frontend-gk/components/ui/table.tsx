import type * as React from "react"
import { cn } from "@/lib/utils"

export function Table({ children }: { children: React.ReactNode }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left text-sm">{children}</table>
    </div>
  )
}

export function THead({ children }: { children: React.ReactNode }) {
  return <thead className="border-b border-border text-[10px] uppercase tracking-widest text-muted-foreground">{children}</thead>
}

export function TBody({ children }: { children: React.ReactNode }) {
  return <tbody className="divide-y divide-border">{children}</tbody>
}

export function TR({ children, className, ...props }: React.ComponentProps<"tr">) {
  return (
    <tr className={cn("transition-colors hover:bg-secondary/50", className)} {...props}>
      {children}
    </tr>
  )
}

export function TH({ children, className }: { children?: React.ReactNode; className?: string }) {
  return <th className={cn("px-4 py-3 font-medium", className)}>{children}</th>
}

export function TD({ children, className, ...props }: React.ComponentProps<"td">) {
  return (
    <td className={cn("px-4 py-3 align-middle", className)} {...props}>
      {children}
    </td>
  )
}
