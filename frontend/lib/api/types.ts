// Backend enum karşılıkları (JsonStringEnumConverter ile string olarak serileşiyor)

export type SellableItemType = "Product" | "Variant"
export type ProductType = "Simple" | "Variant"
export type ClientPlatform = "Web" | "Mobile"
export type UserRole = "Customer" | "Admin"
export type OrderStatus = "Pending" | "Confirmed" | "Cancelled"
export type PaymentStatus = "Succeeded" | "Refunded"
export type ShipmentStatus = "Pending" | "Shipped" | "InTransit" | "Delivered" | "Cancelled"
export type CouponDiscountType = "Percentage" | "FixedAmount"
export type CouponScopeType = "Cart" | "Category" | "Product"

// Ortak

export type PagedResult<T> = {
  items: T[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export type ApiProblem = {
  status?: number
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

// Auth

export type AuthResponse = {
  accessToken: string
  accessTokenExpiresAt: string
  refreshToken: string | null
}

// Catalog

export type ProductAttribute = { id: string; name: string; value: string; displayOrder: number }
export type ProductVariantAttribute = { id: string; name: string; displayOrder: number }
export type ProductVariantOptionValue = { productVariantAttributeId: string; attributeName: string; value: string }
export type ProductVariant = {
  id: string
  sku: string
  isActive: boolean
  optionValues: ProductVariantOptionValue[]
}
export type ProductImage = {
  id: string
  productVariantId: string | null
  url: string
  displayOrder: number
  isPrimary: boolean
}

export type Product = {
  id: string
  name: string
  slug: string
  description: string | null
  categoryId: string
  productType: ProductType
  isActive: boolean
  attributes: ProductAttribute[]
  variantAttributes: ProductVariantAttribute[]
  variants: ProductVariant[]
  images: ProductImage[]
}

export type ProductListItem = {
  id: string
  name: string
  slug: string
  categoryId: string
  productType: ProductType
  isActive: boolean
  primaryImageUrl: string | null
}

export type Category = {
  id: string
  name: string
  slug: string
  description: string | null
  imageUrl: string | null
  parentCategoryId: string | null
  displayOrder: number
  isActive: boolean
}

export type CategoryTree = {
  id: string
  name: string
  slug: string
  description: string | null
  imageUrl: string | null
  displayOrder: number
  isActive: boolean
  children: CategoryTree[]
}

// Cart

export type CartItem = {
  id: string
  sellableItemId: string
  sellableItemType: SellableItemType
  quantity: number
}

export type Cart = {
  id: string
  userId: string | null
  anonymousId: string | null
  items: CartItem[]
}

// Customer

export type CustomerAddress = {
  id: string
  label: string
  recipientName: string
  phoneNumber: string
  country: string
  city: string
  district: string
  postalCode: string
  addressLine1: string
  addressLine2: string | null
  isDefault: boolean
  isShippingAddress: boolean
  isBillingAddress: boolean
}

export type Customer = {
  id: string
  userId: string
  phoneNumber: string | null
  dateOfBirth: string | null
  addresses: CustomerAddress[]
}

export type CustomerListItem = { id: string; userId: string; phoneNumber: string | null; addressCount: number }

export type GuestCustomer = { id: string; firstName: string; lastName: string; email: string; phoneNumber: string }

// Inventory

export type StockItem = {
  id: string
  sellableItemId: string
  sellableItemType: SellableItemType
  quantityOnHand: number
  availableQuantity: number
}

export type StockReservation = {
  id: string
  stockItemId: string
  sellableItemId: string
  sellableItemType: SellableItemType
  referenceId: string
  quantity: number
  expiresAt: string
  isConfirmed: boolean
  isReleased: boolean
  createdAt: string
  confirmedAt: string | null
  releasedAt: string | null
}

// Pricing

export type Price = { id: string; sellableItemId: string; sellableItemType: SellableItemType; amount: number }

export type Coupon = {
  id: string
  code: string
  discountType: CouponDiscountType
  discountValue: number
  maxDiscountAmount: number | null
  minCartAmount: number | null
  scopeType: CouponScopeType
  scopeReferenceId: string | null
  startDate: string
  endDate: string
  totalUsageLimit: number | null
  perUserUsageLimit: number | null
  isActive: boolean
}

export type PriceCalculationItem = { sellableItemId: string; sellableItemType: SellableItemType; quantity: number }

export type PriceCalculationLine = {
  sellableItemId: string
  sellableItemType: SellableItemType
  quantity: number
  unitPrice: number
  lineSubtotal: number
}

export type AppliedCoupon = { code: string; discountAmount: number }

export type PriceCalculationResult = {
  lines: PriceCalculationLine[]
  subtotal: number
  appliedCoupons: AppliedCoupon[]
  totalDiscount: number
  taxRate: number
  taxAmount: number
  grandTotal: number
}

// Orders

export type OrderItem = {
  sellableItemId: string
  sellableItemType: SellableItemType
  quantity: number
  unitPrice: number
  lineSubtotal: number
}

export type OrderCoupon = { code: string; discountAmount: number }

export type Order = {
  id: string
  userId: string | null
  guestCustomerId: string | null
  status: OrderStatus
  cancelledReason: string | null
  recipientName: string
  phoneNumber: string
  country: string
  city: string
  district: string
  postalCode: string
  addressLine1: string
  addressLine2: string | null
  billingRecipientName: string | null
  billingPhoneNumber: string | null
  billingCountry: string | null
  billingCity: string | null
  billingDistrict: string | null
  billingPostalCode: string | null
  billingAddressLine1: string | null
  billingAddressLine2: string | null
  shippingCompanyId: string
  shipmentId: string
  shippingFee: number
  items: OrderItem[]
  coupons: OrderCoupon[]
  subtotal: number
  discountAmount: number
  taxRate: number
  taxAmount: number
  grandTotal: number
  createdAt: string
  updatedAt: string | null
}

// Payment

export type Payment = {
  id: string
  orderId: string
  providerPaymentId: string
  amount: number
  cardAssociation: string | null
  cardFamily: string | null
  cardLastFourDigits: string | null
  status: PaymentStatus
  createdAt: string
  refundedAt: string | null
}

// Review

export type Review = {
  id: string
  userId: string
  sellableItemId: string
  sellableItemType: SellableItemType
  rating: number
  comment: string
  createdAt: string
  updatedAt: string | null
  reviewerDisplayName: string | null
}

export type ReviewSummary = { sellableItemId: string; sellableItemType: SellableItemType; averageRating: number; totalCount: number }

// Shipping

export type ShippingCompany = { id: string; name: string; fee: number; isActive: boolean }

export type ShipmentStatusHistoryEntry = {
  status: ShipmentStatus
  trackingNumber: string | null
  createdAt: string
}

export type Shipment = {
  id: string
  orderId: string
  shippingCompanyId: string
  status: ShipmentStatus
  trackingNumber: string | null
  createdAt: string
  updatedAt: string | null
  history: ShipmentStatusHistoryEntry[]
}

export type ShipmentTracking = {
  id: string
  status: ShipmentStatus
  trackingNumber: string | null
  createdAt: string
  updatedAt: string | null
  history: ShipmentStatusHistoryEntry[]
}

// Notification

export type NotificationLog = {
  id: string
  notificationType: string
  referenceId: string
  recipientEmail: string
  subject: string
  isSuccess: boolean
  errorMessage: string | null
  createdAt: string
}
