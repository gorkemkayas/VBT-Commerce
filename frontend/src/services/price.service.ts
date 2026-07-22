import { apiClient } from '@/lib/api-client';

export const priceService = {
  async getPrice(sellableItemType: number | string, sellableItemId: string): Promise<number> {
    if (!sellableItemId) return 0;
    try {
      const response = await apiClient.get(`/api/prices/${sellableItemType}/${sellableItemId}`);
      return response.data?.amount ?? 0;
    } catch {
      return 0;
    }
  }
};