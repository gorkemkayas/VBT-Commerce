export function formatPrice(value: number) {
  return `₺${value.toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
}

export function formatDate(value: string | Date) {
  const date = typeof value === "string" ? new Date(value) : value
  return date.toLocaleDateString("tr-TR", { year: "numeric", month: "long", day: "numeric" })
}

export function formatDateTime(value: string | Date) {
  const date = typeof value === "string" ? new Date(value) : value
  return date.toLocaleString("tr-TR", { year: "numeric", month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" })
}

export function variantLabel(optionValues: { attributeName: string; value: string }[]) {
  return optionValues.map((v) => v.value).join(" / ")
}
