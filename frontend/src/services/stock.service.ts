import { apiClient } from '@/lib/api-client';

export interface StockItem {
  id: string;
  sellableItemId: string;
  sellableItemType: number;
  quantityOnHand: number;
  availableQuantity: number;
}

export const stockService = {
  // Ürün/Varyant ID'sine göre stok bilgisini getirir
  async getStockBySellableItem(sellableItemId: string, sellableItemType: number = 1): Promise<StockItem | null> {
    try {
      const response = await apiClient.get<StockItem>(
        `/api/stock-items/by-sellable-item/${sellableItemId}?sellableItemType=${sellableItemType}`
      );
      return response.data;
    } catch (error) {
      console.warn('Stok bilgisi alınamadı:', error);
      return null;
    }
  },

  // (İhtiyaç halinde) Stok kaydı oluşturma
  async createStockItem(sellableItemId: string, initialQuantity: number = 100, sellableItemType: number = 1) {
    try {
      const response = await apiClient.post('/api/stock-items', {
        sellableItemId,
        sellableItemType,
        initialQuantity,
      });
      return response.data;
    } catch (error) {
      console.error('Stok oluşturulamadı:', error);
      throw error;
    }
  },

  // (İhtiyaç halinde) Stok artırma (Sıfır olan stoğu düzeltmek için)
  async increaseStock(stockItemId: string, quantity: number) {
    try {
      const response = await apiClient.post(`/api/stock-items/${stockItemId}/increase`, { quantity });
      return response.data;
    } catch (error) {
      console.error('Stok artırılamadı:', error);
      throw error;
    }
  }
};