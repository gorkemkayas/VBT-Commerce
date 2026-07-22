import { apiClient } from '@/lib/api-client';

export interface PlaceOrderPayload {
  addressId: string;
  shippingCompanyId: string;
  couponCodes?: string[];
  cardHolderName: string;
  cardNumber: string;
  cardExpireMonth: string;
  cardExpireYear: string;
  cardCvc: string;
  buyerIdentityNumber: string;
}

class CheckoutService {
  async placeMyOrder(payload: PlaceOrderPayload) {
    try {
      const response = await apiClient.post('/api/orders/me', payload);
      return response.data; // Başarılı olursa Sipariş ID (UUID) döner[cite: 2]
    } catch (error: any) {
      console.error('--- SİPARİŞ OLUŞTURULURKEN HATA ---', error?.response?.data || error);
      throw error;
    }
  }
}

export const checkoutService = new CheckoutService();