import { ChevronLeft, ChevronRight } from "lucide-react"

export function Pagination({
  pageNumber,
  totalPages,
  onChange,
}: {
  pageNumber: number
  totalPages: number
  onChange: (page: number) => void
}) {
  if (totalPages <= 1) return null

  return (
    <div className="flex items-center justify-between border-t border-border pt-4">
      <button
        type="button"
        disabled={pageNumber <= 1}
        onClick={() => onChange(pageNumber - 1)}
        className="flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground disabled:pointer-events-none disabled:opacity-40"
      >
        <ChevronLeft className="h-3.5 w-3.5" strokeWidth={1.5} />
        Önceki
      </button>
      <span className="text-xs tabular-nums text-muted-foreground">
        Sayfa {pageNumber} / {totalPages}
      </span>
      <button
        type="button"
        disabled={pageNumber >= totalPages}
        onClick={() => onChange(pageNumber + 1)}
        className="flex items-center gap-1 text-xs font-medium uppercase tracking-widest text-muted-foreground transition-colors hover:text-foreground disabled:pointer-events-none disabled:opacity-40"
      >
        Sonraki
        <ChevronRight className="h-3.5 w-3.5" strokeWidth={1.5} />
      </button>
    </div>
  )
}
