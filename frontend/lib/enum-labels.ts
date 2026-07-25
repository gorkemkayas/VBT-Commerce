import type { CouponDiscountType, CouponScopeType, OrderStatus, PaymentStatus, ShipmentStatus } from "@/lib/api/types"

export const orderStatusLabels: Record<OrderStatus, string> = {
  Pending: "Beklemede",
  Confirmed: "Onaylandı",
  Cancelled: "İptal Edildi",
}

export const orderStatusTone: Record<OrderStatus, "neutral" | "success" | "danger" | "warning"> = {
  Pending: "warning",
  Confirmed: "success",
  Cancelled: "danger",
}

export const shipmentStatusLabels: Record<ShipmentStatus, string> = {
  Pending: "Hazırlanıyor",
  Shipped: "Kargoya Verildi",
  InTransit: "Dağıtımda",
  Delivered: "Teslim Edildi",
  Cancelled: "İptal Edildi",
}

export const shipmentStatusTone: Record<ShipmentStatus, "neutral" | "success" | "danger" | "warning" | "info"> = {
  Pending: "neutral",
  Shipped: "info",
  InTransit: "warning",
  Delivered: "success",
  Cancelled: "danger",
}

export const paymentStatusLabels: Record<PaymentStatus, string> = {
  Succeeded: "Başarılı",
  Refunded: "İade Edildi",
}

export const paymentStatusTone: Record<PaymentStatus, "success" | "warning"> = {
  Succeeded: "success",
  Refunded: "warning",
}

export const couponDiscountTypeLabels: Record<CouponDiscountType, string> = {
  Percentage: "Yüzde (%)",
  FixedAmount: "Sabit Tutar",
}

export const couponScopeTypeLabels: Record<CouponScopeType, string> = {
  Cart: "Sepet Geneli",
  Category: "Kategori",
  Product: "Ürün",
}
