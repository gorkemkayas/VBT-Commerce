import { apiFetch } from "./client"
import type { Category, CategoryTree, PagedResult, Product, ProductListItem, ProductType } from "./types"

// Kategoriler — herkese açık okuma

export function getCategoryTree(includeInactive = false) {
  return apiFetch<CategoryTree[]>("/api/categories/tree", { auth: false, query: { includeInactive } })
}

export function getCategory(categoryId: string) {
  return apiFetch<Category>(`/api/categories/${categoryId}`, { auth: false })
}

// Kategoriler — admin

export function createCategory(input: {
  name: string
  slug: string
  description?: string | null
  imageUrl?: string | null
  parentCategoryId?: string | null
  displayOrder: number
}) {
  return apiFetch<string>("/api/categories", { method: "POST", body: input })
}

export function updateCategory(
  categoryId: string,
  input: {
    name: string
    slug: string
    description?: string | null
    imageUrl?: string | null
    displayOrder: number
    parentCategoryId?: string | null
  },
) {
  return apiFetch<void>(`/api/categories/${categoryId}`, { method: "PUT", body: input })
}

export function deleteCategory(categoryId: string) {
  return apiFetch<void>(`/api/categories/${categoryId}`, { method: "DELETE" })
}

// Ürünler — herkese açık okuma

export function getProducts(params: {
  pageNumber?: number
  pageSize?: number
  categoryId?: string
  isActive?: boolean
  searchTerm?: string
}) {
  return apiFetch<PagedResult<ProductListItem>>("/api/products", { auth: false, query: params })
}

export function getProductById(productId: string) {
  return apiFetch<Product>(`/api/products/${productId}`, { auth: false })
}

export function getProductBySlug(slug: string) {
  return apiFetch<Product>(`/api/products/by-slug/${slug}`, { auth: false })
}

// Ürünler — admin

export function createProduct(input: { name: string; slug: string; description?: string | null; categoryId: string; productType: ProductType }) {
  return apiFetch<string>("/api/products", { method: "POST", body: input })
}

export function updateProduct(productId: string, input: { name: string; slug: string; description?: string | null; categoryId: string }) {
  return apiFetch<void>(`/api/products/${productId}`, { method: "PUT", body: input })
}

export function deleteProduct(productId: string) {
  return apiFetch<void>(`/api/products/${productId}`, { method: "DELETE" })
}

export function activateProduct(productId: string) {
  return apiFetch<void>(`/api/products/${productId}/activate`, { method: "POST" })
}

export function addProductAttribute(productId: string, input: { name: string; value: string; displayOrder: number }) {
  return apiFetch<string>(`/api/products/${productId}/attributes`, { method: "POST", body: input })
}

export function updateProductAttribute(productId: string, attributeId: string, input: { name: string; value: string; displayOrder: number }) {
  return apiFetch<void>(`/api/products/${productId}/attributes/${attributeId}`, { method: "PUT", body: input })
}

export function removeProductAttribute(productId: string, attributeId: string) {
  return apiFetch<void>(`/api/products/${productId}/attributes/${attributeId}`, { method: "DELETE" })
}

export function addProductVariantAttribute(productId: string, input: { name: string; displayOrder: number }) {
  return apiFetch<string>(`/api/products/${productId}/variant-attributes`, { method: "POST", body: input })
}

export function removeProductVariantAttribute(productId: string, variantAttributeId: string) {
  return apiFetch<void>(`/api/products/${productId}/variant-attributes/${variantAttributeId}`, { method: "DELETE" })
}

export function addProductVariant(productId: string, input: { sku: string; optionValues: Record<string, string> }) {
  return apiFetch<string>(`/api/products/${productId}/variants`, { method: "POST", body: input })
}

export function updateProductVariant(
  productId: string,
  variantId: string,
  input: { sku: string; optionValues: Record<string, string>; isActive: boolean },
) {
  return apiFetch<void>(`/api/products/${productId}/variants/${variantId}`, { method: "PUT", body: input })
}

export function removeProductVariant(productId: string, variantId: string) {
  return apiFetch<void>(`/api/products/${productId}/variants/${variantId}`, { method: "DELETE" })
}

export function addProductImage(
  productId: string,
  input: { url: string; displayOrder: number; isPrimary: boolean; productVariantId?: string | null },
) {
  return apiFetch<string>(`/api/products/${productId}/images`, { method: "POST", body: input })
}

export function removeProductImage(productId: string, imageId: string) {
  return apiFetch<void>(`/api/products/${productId}/images/${imageId}`, { method: "DELETE" })
}

export function setPrimaryProductImage(productId: string, imageId: string) {
  return apiFetch<void>(`/api/products/${productId}/images/${imageId}/set-primary`, { method: "POST" })
}
