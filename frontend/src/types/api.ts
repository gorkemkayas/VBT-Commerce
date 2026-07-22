// ==========================================
// ENUM TANIMLARI
// ==========================================

// Client Platform (0: Web, 1: Mobile)
export enum ClientPlatform {
  Web = 0,
  Mobile = 1,
}

export enum CartItemType {
  Product = 0,
  Variant = 1,
}

export enum ProductType {
  Simple = 0,
  Configurable = 1,
}

export enum PriceItemType {
  Product = 0,
  Variant = 1,
}

export enum InventoryItemType {
  Product = 0,
  Variant = 1,
}

export enum ReviewItemType {
  Product = 0,
  Variant = 1,
}

export enum OrderItemType {
  Product = 0,
  Variant = 1,
}

export enum OrderStatus {
  Pending = 0,
  Processing = 1,
  Shipped = 2,
  Completed = 3,
  Cancelled = 4,
}

export enum PaymentStatus {
  Pending = 0,
  Success = 1,
  Failed = 2,
  Refunded = 3,
}

export enum ShipmentStatus {
  Preparing = 0,
  OnTheWay = 1,
  Delivered = 2,
  Failed = 3,
}

export enum CouponDiscountType {
  Percentage = 0,
  FixedAmount = 1,
}

export enum CouponScopeType {
  Global = 0,
  Category = 1,
  Product = 2,
}

// ==========================================
// AUTH & KULLANICI MODELLERİ
// ==========================================

export interface LoginRequest {
  email: string;
  password: string;
  platform: ClientPlatform | number;
  anonymousId?: string | null;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  platform: ClientPlatform | number;
  anonymousId?: string | null;
}

export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken?: string | null;
}

export interface CustomerAddressDto {
  id: string;
  label: string;
  recipientName: string;
  phoneNumber: string;
  country: string;
  city: string;
  district: string;
  postalCode: string;
  addressLine1: string;
  addressLine2?: string | null;
  isDefault: boolean;
}

export interface CustomerDto {
  id: string;
  userId: string;
  phoneNumber?: string | null;
  dateOfBirth?: string | null;
  addresses: CustomerAddressDto[];
}

export interface CustomerListItemDto {
  id: string;
  userId: string;
  phoneNumber?: string | null;
  addressCount: number;
}

// ==========================================
// KATEGORİ MODELLERİ
// ==========================================

export interface CategoryDto {
  id: string;
  name: string;
  slug: string;
  description?: string | null;
  parentCategoryId?: string | null;
  displayOrder: number;
  isActive: boolean;
}

export interface CategoryTreeDto {
  id: string;
  name: string;
  slug: string;
  description?: string | null;
  displayOrder: number;
  isActive: boolean;
  children: CategoryTreeDto[];
}

// ==========================================
// ÜRÜN MODELLERİ
// ==========================================

export interface ProductAttributeDto {
  id: string;
  name: string;
  value: string;
  displayOrder: number;
}

export interface ProductVariantAttributeDto {
  id: string;
  name: string;
  displayOrder: number;
}

export interface ProductVariantOptionValueDto {
  productVariantAttributeId: string;
  attributeName: string;
  value: string;
}

export interface ProductVariantDto {
  id: string;
  sku: string;
  isActive: boolean;
  optionValues: ProductVariantOptionValueDto[];
}

export interface ProductImageDto {
  id: string;
  productVariantId?: string | null;
  url: string;
  displayOrder: number;
  isPrimary: boolean;
}

export interface ProductDto {
  id: string;
  name: string;
  slug: string;
  description?: string | null;
  categoryId: string;
  categoryName?: string;
  productType: ProductType | number;
  isActive: boolean;
  price?: number; // Ekstra uyumluluk için
  attributes: ProductAttributeDto[];
  variantAttributes: ProductVariantAttributeDto[];
  variants: ProductVariantDto[];
  images: ProductImageDto[];
}

export interface ProductListItemDto {
  id: string;
  name: string;
  slug: string;
  categoryId: string;
  categoryName?: string;
  productType: ProductType | number;
  isActive: boolean;
  price?: number;
  primaryImageUrl?: string | null;
}

// Takma Adlar (Geriye Dönük Uyumluluk İçin)
export type Product = ProductDto;

// ==========================================
// SEPET MODELLERİ
// ==========================================

export interface CartItemDto {
  id: string;
  sellableItemId: string;
  sellableItemType: CartItemType | number;
  quantity: number;
}

export interface CartDto {
  id: string;
  userId?: string | null;
  anonymousId?: string | null;
  items: CartItemDto[];
}

export interface AddCartItemRequest {
  sellableItemId: string;
  sellableItemType: CartItemType | number;
  quantity: number;
}

// ==========================================
// SİPARİŞ & FİYATLANDIRMA MODELLERİ
// ==========================================

export interface OrderItemDto {
  sellableItemId: string;
  sellableItemType: OrderItemType | number;
  quantity: number;
  unitPrice: number;
  lineSubtotal: number;
}

export interface OrderCouponDto {
  code: string;
  discountAmount: number;
}

export interface OrderDto {
  id: string;
  userId?: string | null;
  guestCustomerId?: string | null;
  status: OrderStatus | number;
  cancelledReason?: string | null;
  recipientName: string;
  phoneNumber: string;
  country: string;
  city: string;
  district: string;
  postalCode: string;
  addressLine1: string;
  addressLine2?: string | null;
  shippingCompanyId: string;
  shipmentId: string;
  shippingFee: number;
  items: OrderItemDto[];
  coupons: OrderCouponDto[];
  subtotal: number;
  discountAmount: number;
  taxRate: number;
  taxAmount: number;
  grandTotal: number;
  createdAt: string;
  updatedAt?: string | null;
}

// ==========================================
// SAYFALANMIŞ YANIT (PAGED RESULT)
// ==========================================

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type PagedResultOfProductListItemDto = PagedResult<ProductListItemDto>;
export type PagedResultOfOrderDto = PagedResult<OrderDto>;
export type PagedResultOfReviewDto = PagedResult<any>;