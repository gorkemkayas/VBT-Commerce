import { apiClient, getOrCreateAnonymousId, getAccessToken } from '@/lib/api-client';

export interface AddCartItemPayload {
  sellableItemId: string;
  sellableItemType?: number; // 0: Product, 1: Variant
  quantity: number;
}

export const cartService = {
  async addItem(item: AddCartItemPayload) {
    const token = getAccessToken();
    const anonId = getOrCreateAnonymousId();

    const payload = {
      sellableItemId: item.sellableItemId,
      sellableItemType: item.sellableItemType ?? 0, // Varsayılan 0 (Product) olmalı
      quantity: item.quantity || 1,
    };

    try {
      if (token) {
        // Login olmuş kullanıcı
        const res = await apiClient.post('/api/carts/me/items', payload);
        return res.data;
      } else {
        // Misafir kullanıcı
        if (!anonId) throw new Error('Anonymous ID üretilemedi.');
        const res = await apiClient.post(`/api/carts/anonymous/${anonId}/items`, payload);
        return res.data;
      }
    } catch (error: any) {
      console.error('--- SEPETE EKLENİRKEN HATA ---', error?.response?.data || error);
      throw error;
    }
  },

  async getMyCart() {
    const token = getAccessToken();
    const anonId = getOrCreateAnonymousId();

    try {
      if (token) {
        const res = await apiClient.get('/api/carts/me');

        // Giriş yapmış kullanıcının sepeti boşsa ve anonim sepeti varsa ürünleri aktar
        if ((!res.data || !res.data.items || res.data.items.length === 0) && anonId) {
          const anonRes = await apiClient.get(`/api/carts/anonymous/${anonId}`).catch(() => null);
          const anonItems = anonRes?.data?.items;

          // Güvenli Array kontrolü (TypeScript tip hatasını çözer)
          if (Array.isArray(anonItems) && anonItems.length > 0) {
            for (const anonItem of anonItems as any[]) {
              await apiClient.post('/api/carts/me/items', {
                sellableItemId: anonItem.sellableItemId,
                sellableItemType: anonItem.sellableItemType ?? 0,
                quantity: anonItem.quantity,
              }).catch(() => null);
            }
            // Aktarımdan sonra üye sepetini tekrar çek
            const updatedRes = await apiClient.get('/api/carts/me');
            return updatedRes.data || { items: [] };
          }
        }
        return res.data || { items: [] };
      } else if (anonId) {
        const res = await apiClient.get(`/api/carts/anonymous/${anonId}`);
        return res.data || { items: [] };
      }
      return { items: [] };
    } catch (error) {
      return { items: [] };
    }
  },

  async removeItem(cartItemId: string) {
    const token = getAccessToken();
    const anonId = getOrCreateAnonymousId();

    try {
      const endpoint = token 
        ? `/api/carts/me/items/${cartItemId}`
        : `/api/carts/anonymous/${anonId}/items/${cartItemId}`;

      const res = await apiClient.delete(endpoint);
      return res.data;
    } catch (error) {
      console.error('Ürün silinemedi:', error);
      throw error;
    }
  }
};