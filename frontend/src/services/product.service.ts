import { apiClient } from '@/lib/api-client';
import { Product } from '@/types/api';
import { priceService } from './price.service';

export const productService = {
  async getProducts(): Promise<Product[]> {
    try {
      const response = await apiClient.get('/api/products');
      const items = response.data?.items || (Array.isArray(response.data) ? response.data : []);

      const enrichedProducts = await Promise.all(
        items.map(async (item: any) => {
          let imageUrl = item.primaryImageUrl || null;

          // Ana Ekranda Görünecek Fiyat: Önce Type 0 (Product) ile sorgula
          let price = await priceService.getPrice(0, item.id);

          // Eğer Type 0 fiyatı tanımlı değilse Type 1 (Variant) dene
          if (price === 0) {
            price = await priceService.getPrice(1, item.id);
          }

          // Resim veya fiyat hala eksikse detaydan tamamla
          if (price === 0 || !imageUrl) {
            try {
              const detailRes = await apiClient.get(`/api/products/${item.id}`);
              const productDetail = detailRes.data;

              if (!imageUrl && productDetail?.images?.length > 0) {
                const primaryImg = productDetail.images.find((img: any) => img.isPrimary) || productDetail.images[0];
                imageUrl = primaryImg?.url || null;
              }

              if (price === 0 && productDetail?.variants?.length > 0) {
                for (const variant of productDetail.variants) {
                  price = await priceService.getPrice(1, variant.id);
                  if (price > 0) break;
                }
              }
            } catch {
              /* ignore */
            }
          }

          return {
            id: item.id,
            name: item.name,
            slug: item.slug,
            price: price,
            categoryName: item.categoryName || 'Lüks Koleksiyon',
            images: imageUrl ? [{ id: '1', url: imageUrl, isPrimary: true }] : [],
          } as Product;
        })
      );

      return enrichedProducts;
    } catch (error) {
      console.error('API getProducts hatası:', error);
      return [];
    }
  },

  async getProductBySlug(slug: string): Promise<Product | null> {
    try {
      const response = await apiClient.get(`/api/products/by-slug/${slug}`);
      const product = response.data;
      if (!product) return null;

      // Taban Fiyat: Type 0 (Ana Ürün Fiyatı)
      let basePrice = await priceService.getPrice(0, product.id);
      product.price = basePrice;

      return product;
    } catch (error) {
      console.error(`API getProductBySlug hatası (${slug}):`, error);
      return null;
    }
  },
};