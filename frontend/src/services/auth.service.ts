import { apiClient, setAccessToken } from '@/lib/api-client';
import { LoginRequest, RegisterRequest, AuthResponse, CustomerDto, ClientPlatform } from '@/types/api';

export const authService = {
  async register(data: Omit<RegisterRequest, 'platform' | 'anonymousId'>): Promise<AuthResponse> {
    const anonId = typeof window !== 'undefined' ? localStorage.getItem('anonymous_id') : null;

    const res = await apiClient.post<AuthResponse>('/api/auth/register', {
      ...data,
      platform: ClientPlatform.Web, // Integer enum 0 gönderilir
      anonymousId: anonId || null,
    });

    if (res.data?.accessToken) {
      setAccessToken(res.data.accessToken);
    }
    return res.data;
  },

  async login(data: Omit<LoginRequest, 'platform' | 'anonymousId'>): Promise<AuthResponse> {
    const anonId = typeof window !== 'undefined' ? localStorage.getItem('anonymous_id') : null;

    const res = await apiClient.post<AuthResponse>('/api/auth/login', {
      ...data,
      platform: ClientPlatform.Web, // Integer enum 0 gönderilir
      anonymousId: anonId || null,
    });

    if (res.data?.accessToken) {
      setAccessToken(res.data.accessToken);
    }
    return res.data;
  },

  async logout(): Promise<void> {
    try {
      await apiClient.post('/api/auth/logout', {});
    } catch {
      /* ignore */
    } finally {
      setAccessToken(null);
    }
  },

  // Swagger Spesifikasyonuna Uyumlu: /api/customers/me
  async getCurrentCustomer(): Promise<CustomerDto> {
    const res = await apiClient.get<CustomerDto>('/api/customers/me');
    return res.data;
  },

  // HATA BURADAYDI: Objenin içine taşındı ve doğru formata getirildi
  async refreshToken(): Promise<string | null> {
    try {
      const res = await apiClient.post<AuthResponse>('/api/auth/refresh', {
        platform: ClientPlatform.Web,
      });

      if (res.data?.accessToken) {
        setAccessToken(res.data.accessToken);
        return res.data.accessToken;
      }
      return null;
    } catch {
      setAccessToken(null);
      return null;
    }
  },
};